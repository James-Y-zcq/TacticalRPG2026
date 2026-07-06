using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Generates a procedural city layout on a virtual grid, then uses StructurePlacer to build it.
/// </summary>
[RequireComponent(typeof(StructurePlacer))]
public class ProceduralCityGenerator : MonoBehaviour
{
    public enum CellType { Empty, River, Wall, Gate, Road, Building }
    public enum RiverDirection { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

    [Header("City Dimensions (Must be multiples of 4)")]
    public int cityWidth = 64;
    public int cityLength = 64;

    [Header("River Settings")]
    [SerializeField] bool riverEnabled = false;
    public RiverDirection riverDirection = RiverDirection.North;
    [Range(0f, 1f)]
    [Tooltip("Probability of the river turning perpendicularly. Diagonals meander automatically.")]
    public float riverMeanderChance = 0.2f;

    [Header("4x4 Chunk Data")]
    public StructureData riverChunk;
    public StructureData wallChunk;
    public StructureData gatehouseChunk;
    public StructureData roadChunk;
    public StructureData bridgeChunk;

    [Header("Buildings")]
    public List<StructureData> buildingPrefabs;
    [Tooltip("How many times to attempt placing a building before giving up.")]
    public int placementAttemptsPerBuilding = 50;
    public int totalBuildingsToSpawn = 30;

    private StructurePlacer placer;
    private CellType[,] cityGrid;

    // A queue of instructions to hand to the Placer once the math is done
    private struct PlacementJob
    {
        public StructureData data;
        public Vector3Int position;
    }
    private List<PlacementJob> placementQueue = new List<PlacementJob>();

    private void Awake()
    {
        placer = GetComponent<StructurePlacer>();
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        if (placer == null) placer = GetComponent<StructurePlacer>();

        //Init the empty grid
        cityGrid = new CellType[cityWidth, cityLength];
        placementQueue.Clear();

        // Clear existing tilemaps before generating
        foreach (var tm in placer.targetTilemaps)
        {
            if (tm != null) tm.ClearAllTiles();
        }

        //Determine placements internally
        GenerateRiver();
        GenerateWallsAndGates();
        GenerateMainRoads();
        GenerateBuildings();

        //Execute the placement queue to paint the map visually
        foreach (PlacementJob job in placementQueue)
        {
            placer.PlaceStructure(job.data, job.position);
        }

        Debug.Log($"City generated with {placementQueue.Count} total structures.");
    }

    private void GenerateRiver()
    {
        if(!riverEnabled) return;

        int startX = 0, startY = 0;
        
        // Calculate the maximum chunk index limits
        int maxW = (cityWidth / 4) - 1;
        int maxH = (cityLength / 4) - 1;

        // 1. Determine starting location based on overall flow direction
        switch (riverDirection)
        {
            case RiverDirection.North:
                startX = Random.Range(2, maxW - 1) * 4;
                startY = 0;
                break;
            case RiverDirection.South:
                startX = Random.Range(2, maxW - 1) * 4;
                startY = maxH * 4;
                break;
            case RiverDirection.East:
                startX = 0;
                startY = Random.Range(2, maxH - 1) * 4;
                break;
            case RiverDirection.West:
                startX = maxW * 4;
                startY = Random.Range(2, maxH - 1) * 4;
                break;
            case RiverDirection.NorthEast:
                if (Random.value < 0.5f) { startX = Random.Range(0, maxW / 4) * 4; startY = 0; }
                else { startX = 0; startY = Random.Range(0, maxH / 4) * 4; }
                break;
            case RiverDirection.NorthWest:
                if (Random.value < 0.5f) { startX = Random.Range(maxW - (maxW / 4), maxW) * 4; startY = 0; }
                else { startX = maxW * 4; startY = Random.Range(0, maxH / 4) * 4; }
                break;
            case RiverDirection.SouthEast:
                if (Random.value < 0.5f) { startX = Random.Range(0, maxW / 4) * 4; startY = maxH * 4; }
                else { startX = 0; startY = Random.Range(maxH - (maxH / 4), maxH) * 4; }
                break;
            case RiverDirection.SouthWest:
                if (Random.value < 0.5f) { startX = Random.Range(maxW - (maxW / 4), maxW) * 4; startY = maxH * 4; }
                else { startX = maxW * 4; startY = Random.Range(maxH - (maxH / 4), maxH) * 4; }
                break;
        }

        int x = startX;
        int y = startY;
        Vector2Int lastStep = Vector2Int.zero;
        int failsafe = 0;

        // 2. Plot the river course until it wanders completely off the grid limits
        while (x >= 0 && x < cityWidth && y >= 0 && y < cityLength && failsafe < 1000)
        {
            failsafe++;

            // Claim the 4x4 spot if it hasn't been claimed by overlapping river bends
            if (cityGrid[x, y] != CellType.River)
            {
                MarkGridAndQueue(x, y, 4, 4, CellType.River, riverChunk);
            }

            Vector2Int step = Vector2Int.zero;
            int attempt = 0;
            do
            {
                step = GetNextRiverStep(riverDirection);
                attempt++;
            }
            while (attempt < 10 && step == -lastStep); // Prevent it from instantly turning 180 degrees back on itself

            lastStep = step;
            x += step.x;
            y += step.y;
        }
    }

    private Vector2Int GetNextRiverStep(RiverDirection dir)
    {
        float r = Random.value;

        switch (dir)
        {
            case RiverDirection.North:
                if (r < riverMeanderChance / 2f) return new Vector2Int(-4, 0); // Wobble Left
                if (r < riverMeanderChance) return new Vector2Int(4, 0);      // Wobble Right
                return new Vector2Int(0, 4);                                  // Main direction

            case RiverDirection.South:
                if (r < riverMeanderChance / 2f) return new Vector2Int(-4, 0);
                if (r < riverMeanderChance) return new Vector2Int(4, 0);
                return new Vector2Int(0, -4);

            case RiverDirection.East:
                if (r < riverMeanderChance / 2f) return new Vector2Int(0, -4);
                if (r < riverMeanderChance) return new Vector2Int(0, 4);
                return new Vector2Int(4, 0);

            case RiverDirection.West:
                if (r < riverMeanderChance / 2f) return new Vector2Int(0, -4);
                if (r < riverMeanderChance) return new Vector2Int(0, 4);
                return new Vector2Int(-4, 0);

            // Diagonals meander naturally by randomly deciding which orthogonal step to take next.
            // This naturally produces a "staircase" path moving in the diagonal direction!
            case RiverDirection.NorthEast:
                return Random.value < 0.5f ? new Vector2Int(4, 0) : new Vector2Int(0, 4);
            case RiverDirection.NorthWest:
                return Random.value < 0.5f ? new Vector2Int(-4, 0) : new Vector2Int(0, 4);
            case RiverDirection.SouthEast:
                return Random.value < 0.5f ? new Vector2Int(4, 0) : new Vector2Int(0, -4);
            case RiverDirection.SouthWest:
                return Random.value < 0.5f ? new Vector2Int(-4, 0) : new Vector2Int(0, -4);
        }

        return new Vector2Int(0, 4); // Fallback
    }

    private void GenerateWallsAndGates()
    {
        // 1. Calculate macro chunk boundary coordinates (Multiples of 4)
        int minMacroX = 4;
        int maxMacroX = (cityWidth / 4) - 2; // Last chunk index available for walls
        int minMacroY = 4;
        int maxMacroY = (cityLength / 4) - 2;

        // 2. Convert those chunk origins into absolute tile coordinates
        int minX = minMacroX * 4;
        int maxX = maxMacroX * 4;
        int minY = minMacroY * 4;
        int maxY = maxMacroY * 4;

        // 3. Find the midpoints for the gates (must remain snapped to 4x4)
        int midX = ((minMacroX + maxMacroX) / 2) * 4;
        int midY = ((minMacroY + maxMacroY) / 2) * 4;

        // --- Bottom and Top Walls ---
        for (int x = minX; x <= maxX; x += 1) 
        {
            // Protect the entire 4-tile span where the gatehouse sits
            if (x >= midX && x < midX + 4)
            {
                if (x == midX)
                {
                    MarkGridAndQueue(x, minY, 4, 4, CellType.Gate, gatehouseChunk);
                    MarkGridAndQueue(x, maxY, 4, 4, CellType.Gate, gatehouseChunk);
                }
            }
            else
            {
                // Place continuous 1x1 walls if there isn't a river blocking the spot
                if (cityGrid[x, minY] != CellType.River) MarkGridAndQueue(x, minY, 1, 1, CellType.Wall, wallChunk);
                if (cityGrid[x, maxY] != CellType.River) MarkGridAndQueue(x, maxY, 1, 1, CellType.Wall, wallChunk);
            }
        }

        // --- Left and Right Walls ---
        for (int y = minY + 1; y < maxY; y += 1) // Start at +1 to avoid corner overlaps
        {
            // Protect the entire 4-tile span where the side gatehouse sits
            if (y >= midY && y < midY + 4)
            {
                if (y == midY)
                {
                    MarkGridAndQueue(minX, y, 4, 4, CellType.Gate, gatehouseChunk);
                    MarkGridAndQueue(maxX, y, 4, 4, CellType.Gate, gatehouseChunk);
                }
            }
            else
            {
                // Check if the area is empty before placing to prevent overlapping corners
                if (cityGrid[minX, y] == CellType.Empty) MarkGridAndQueue(minX, y, 1, 1, CellType.Wall, wallChunk);
                if (cityGrid[maxX, y] == CellType.Empty) MarkGridAndQueue(maxX, y, 1, 1, CellType.Wall, wallChunk);
            }
        }
    }

    private void GenerateMainRoads()
    {
        // 1. Calculate macro chunk boundary coordinates (identical to walls)
        int minMacroX = 4;
        int maxMacroX = (cityWidth / 4) - 2;
        int minMacroY = 4;
        int maxMacroY = (cityLength / 4) - 2;

        // 2. Find the exact same midpoints
        int midX = ((minMacroX + maxMacroX) / 2) * 4;
        int midY = ((minMacroY + maxMacroY) / 2) * 4;

        // 3. Roads start just inside the gates
        int minX = (minMacroX + 1) * 4;
        int maxX = (maxMacroX - 1) * 4;
        int minY = (minMacroY + 1) * 4;
        int maxY = (maxMacroY - 1) * 4;

        // Vertical Road
        for (int y = minY; y <= maxY; y += 4)
        {
            if (cityGrid[midX, y] == CellType.River)
                MarkGridAndQueue(midX, y, 4, 4, CellType.Road, bridgeChunk); // Bridge over river
            else if (cityGrid[midX, y] == CellType.Empty)
                MarkGridAndQueue(midX, y, 4, 4, CellType.Road, roadChunk);
        }

        // Horizontal Road
        for (int x = minX; x <= maxX; x += 4)
        {
            if (cityGrid[x, midY] == CellType.River)
                MarkGridAndQueue(x, midY, 4, 4, CellType.Road, bridgeChunk); // Bridge over river
            else if (cityGrid[x, midY] == CellType.Empty)
                MarkGridAndQueue(x, midY, 4, 4, CellType.Road, roadChunk);
        }
    }

    private void GenerateBuildings()
    {
        if (buildingPrefabs == null || buildingPrefabs.Count == 0) return;

        // Clean out any empty Inspector slots to prevent Null Reference Exceptions
        buildingPrefabs.RemoveAll(prefab => prefab == null);

        // Sort buildings from largest footprint to smallest. 
        // Placing big buildings first is the secret to good procedural generation!
        buildingPrefabs.Sort((StructureData a, StructureData b) => 
        {
            float areaA = a.footprint.x * a.footprint.y;
            float areaB = b.footprint.x * b.footprint.y;
            return areaB.CompareTo(areaA);
        });

        int buildingsPlaced = 0;

        for (int i = 0; i < totalBuildingsToSpawn; i++)
        {
            // Pick a random building from our list
            StructureData building = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
            int w = (int)building.footprint.x;
            int h = (int)building.footprint.y;

            bool placed = false;

            // Try to find a spot N times
            for (int attempt = 0; attempt < placementAttemptsPerBuilding; attempt++)
            {
                // Pick a random internal coordinate (inside the walls)
                int x = Random.Range(8, cityWidth - 8 - w);
                int y = Random.Range(8, cityLength - 8 - h);

                if (CheckAreaEmpty(x, y, w, h))
                {
                    // Success! It fits perfectly.
                    MarkGridAndQueue(x, y, w, h, CellType.Building, building);
                    placed = true;
                    buildingsPlaced++;
                    break; 
                }
            }
            
            // If we couldn't place it after many attempts, the city might be getting full.
        }
    }

    /// <summary>
    /// Checks if a mathematical rectangle on the grid is entirely empty.
    /// </summary>
    private bool CheckAreaEmpty(int startX, int startY, int width, int height)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                if (cityGrid[x, y] != CellType.Empty) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Claims the space on the mathematical grid, and adds the structure to the placement queue.
    /// </summary>
    private void MarkGridAndQueue(int startX, int startY, int width, int height, CellType type, StructureData data)
    {
        if (data == null) return;

        // 1. Claim the area on our virtual math grid so nothing else spawns here
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                cityGrid[x, y] = type;
            }
        }

        // 2. Add to queue for the Placer to handle visually later
        placementQueue.Add(new PlacementJob
        {
            data = data,
            position = new Vector3Int(startX, startY, 0)
        });
    }
}