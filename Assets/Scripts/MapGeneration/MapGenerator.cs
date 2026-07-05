using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<CityBuilding> roadList;
    [SerializeField] StructurePlacer structurePlacer; //grab a reference to do the placements
    public void GenerateCity(MapCity city)
    {
        //first determine the footprint of the city so roads can be created, and then walls.
        Vector2 dimensions = new Vector2();
        foreach(var building in city.buildings)
        {
            dimensions.x += building.BuildingData.footprint.x;
            dimensions.y += building.BuildingData.footprint.y;
        }

        //add a buffer for roads (leave enough for a city block per building. This will change later based on settlement development)
        dimensions.x += city.buildings.Count * 4;
        dimensions.y += city.buildings.Count * 4;

        //now time to draw roads using the city's development level. This will be a simple grid for now, but will be more complex later.
        int gridSize = (int)Mathf.Sqrt(city.buildings.Count);

        //use the structure placer to place 8x8 road segments in a grid.

    }

    /// <summary>
    /// Creates the terrain for a battle map. Cities are placed on top of terrain.
    /// </summary>
    public void GenerateTerrain()
    {
        
    }
}