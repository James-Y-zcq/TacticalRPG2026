using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MapCity is the map depiction of each city. All cities are procedurally generated on the battlefield. MapCity stores population info, buildings, etc. This is a vital class to the campaign architecture.
/// </summary>
public class MapCity : MonoBehaviour
{
    public int recruitablePopulation {get; private set;}
    [SerializeField] public List<CityBuilding> buildings;

    /// <summary>
    /// sets up a city for use on the campaign map
    /// </summary>
    public void initCity()
    {
        buildings = new List<CityBuilding>(); //initialize the 
    }
}