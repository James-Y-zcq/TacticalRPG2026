using System.Collections.Generic;
using UnityEngine;

public class MapFactionBase : ScriptableObject
{
    [SerializeField] string factionTag;
    [SerializeField] string factionName;
    [SerializeField] Sprite factionBanner;
    [SerializeField] List<string> startingRegions;
    [SerializeField] Color mapColor;
    [SerializeField] Color secondaryColor;
    [SerializeField] CultureType factionCulture;

    //getters
    public string FactionTag => factionTag;
    public string FactionName => factionName;
    public Sprite FactionBanner => factionBanner;
    public List<string> StartingRegions => startingRegions;
    public Color MapColor => mapColor;
    public Color SecondaryColor => secondaryColor;
    public CultureType FactionCulture => factionCulture;
}
public enum CultureType
{
    Dolebian,
    Livernic,
    Opterian,
    Saffrontyr
}