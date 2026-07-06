using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StructurePlacer))]
public class MapGenerator : MonoBehaviour
{
    [Header("City Dimensions (Must be multiple of 4)")]
    [SerializeField] int cityWidth = 64;
    [SerializeField] int cityLength = 64;

    [Header("Chunk Data")]
    public StructureData riverChunk;
    public StructureData wallChunk;
    public StructureData gatehouseChunk;
    public StructureData roadChunk;
    public StructureData bridgeChunk;

    [Header("Buildings")]
    public List<StructureData> buildingPrefabs;
    [Tooltip("How many times to attempt to place a building before abandoning the placement")]
    public int placementAttemptsPerBuilding = 50;
    public int totalBuildingsToSpawn = 30;

    private StructurePlacer placer;
    private CellType[,] cityGrid;

    //A discrete instruction used in a queue for placements
    private struct PlacementJob
    {
        public StructureData data;
        public Vector3Int position;
    }

    private List<PlacementJob> placementQueue = new List<PlacementJob>();

    void Awake()
    {
        placer = GetComponent<StructurePlacer>();
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        if (placer == null) placer = GetComponent<StructurePlacer>();

        //Setup the empty grid
        cityGrid = new CellType[cityWidth, cityLength];
        placementQueue.Clear();

        // Clear tilemaps before generating
        placer.clearTestTilemaps();

        GenerateRiver();
        GenerateWallsAndGates();
        GenerateMainRoads();
        GenerateBuildings();

        //Do the placements
        foreach (PlacementJob job in placementQueue)
        {
            placer.PlaceStructure(job.data, job.position);
        }

        Debug.Log($"City generated with {placementQueue.Count} total structures.");
    }

    private void GenerateRiver()
    {
        // Pick a random X coordinate for the river, snapping it to a 4-block grid
        int riverStartX = Random.Range(4, (cityWidth / 4) - 4) * 4;

        // Draw a straight river from bottom (y=0) to top (y=cityLength)
        for (int y = 0; y < cityLength; y += 4)
        {
            MarkGridAndQueue(riverStartX, y, 4, 4, CellType.River, riverChunk);
        }
    }

    private void GenerateWallsAndGates()
    {
        // Mark perimeter
        int minX = 4, maxX = cityWidth - 8;
        int minY = 4, maxY = cityLength - 8;

        // Find midpoints
        int midX = ((minX + maxX) / 2 / 4) * 4;
        int midY = ((minY + maxY) / 2 / 4) * 4;

        // Bottom and Top Walls
        for (int x = minX; x <= maxX; x += 4)
        {
            if (x == midX) 
            {
                MarkGridAndQueue(x, minY, 4, 4, CellType.Gate, gatehouseChunk);
                MarkGridAndQueue(x, maxY, 4, 4, CellType.Gate, gatehouseChunk);
            }
            else
            {
                // Only place wall if there isn't a river here!
                if (cityGrid[x, minY] != CellType.River) MarkGridAndQueue(x, minY, 4, 4, CellType.Wall, wallChunk);
                if (cityGrid[x, maxY] != CellType.River) MarkGridAndQueue(x, maxY, 4, 4, CellType.Wall, wallChunk);
            }
        }

        // Left and Right Walls
        for (int y = minY + 4; y < maxY; y += 4) // +4 to avoid corners overlapping
        {
            if (y == midY)
            {
                MarkGridAndQueue(minX, y, 4, 4, CellType.Gate, gatehouseChunk);
                MarkGridAndQueue(maxX, y, 4, 4, CellType.Gate, gatehouseChunk);
            }
            else
            {
                MarkGridAndQueue(minX, y, 4, 4, CellType.Wall, wallChunk);
                MarkGridAndQueue(maxX, y, 4, 4, CellType.Wall, wallChunk);
            }
        }
    }

    private void GenerateMainRoads()
    {
        int minX = 8, maxX = cityWidth - 12;
        int minY = 8, maxY = cityLength - 12;
        int midX = ((4 + cityWidth - 8) / 2 / 4) * 4;
        int midY = ((4 + cityLength - 8) / 2 / 4) * 4;

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

        // Sort buildings from largest footprint to smallest. 
        // Placing big buildings first is the secret to good procedural generation!
        buildingPrefabs.Sort((a, b) => (b.footprint.x * b.footprint.y).CompareTo(a.footprint.x * a.footprint.y));

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
    /// Checks if a footprint on the grid is entirely empty.
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
    /// Marks the space on the mathematical grid and adds the structure to the placement queue.
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

public enum CellType
{
    Empty,
    River,
    Wall,
    Gate,
    Road,
    Building,
}