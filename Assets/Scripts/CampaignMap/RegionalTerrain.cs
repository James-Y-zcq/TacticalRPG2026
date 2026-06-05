using UnityEngine;

[CreateAssetMenu(menuName = "Map/Regional Terrain")]
public class RegionalTerrain : ScriptableObject
{
    [SerializeField] Material regionalMaterial;
    [SerializeField] string terrainName;
    [SerializeField] Color backgroundColor;

    public Material RegionalMaterial { get { return regionalMaterial; } }
    public string TerrainName { get { return terrainName; } }
    public Color BackgroundColor { get { return backgroundColor; } }
}