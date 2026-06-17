using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleFieldBuilding/Create new BattlefieldBuilding")]
public class BattlefieldBuilding : ScriptableObject
{
    /// <summary>
    /// Tiles that are used in this building. Allows for palette swapping easily to make a building a different culture.
    /// </summary>
    [SerializeField] List<Sprite> usedTiles;

    /// <summary>
    /// List of 2d arrays that represents the indices of the associated tiles on a layer. A development tool will likely be required to accomodate easier construction.
    /// </summary>
    [SerializeField] List<int[][]> layers;
    
    /// <summary>
    /// Dimensions of the building, in tiles.
    /// </summary>
    [SerializeField] Vector2 footprint;
}