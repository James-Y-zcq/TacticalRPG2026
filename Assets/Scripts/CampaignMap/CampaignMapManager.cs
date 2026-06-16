using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CampaignMapManager : MonoBehaviour
{
    #region fields
    public static CampaignMapManager i;
    [SerializeField] List<Region> regions;
    [SerializeField] List<MapFactionBase> factionBases;
    private List<MapFaction> factions;
    private MapFaction playerFaction;
    private int turnCount = 0; //starts at zero
    private Region highlightedRegion;

    [SerializeField] Transform unitParent;
    [SerializeField] Transform cityParent;

    [Header("Globally Accessible Fields")]
    public List<Region> regionList => regions;
    [SerializeField] Material defaultRegionMaterial; //should just be sprite unlit default
    public Material DefaultRegionMaterial => defaultRegionMaterial;
    public FieldArmy selectedArmy {get; private set;} //an army that is currently selected
    public MapCity selectedCity {get; private set;} //a settlement that is currently selected

    public List<FieldArmy> fieldArmies {get; private set;}
    public List<MapCity> cities {get; private set;}
    #endregion

    #region Helper Scripts
    [SerializeField] CampaignUI campaignUI;
    #endregion

    void Awake()
    {
        if (i == null)
            i = this;

        SetupCampaign("BRE");

        campaignUI.updateYearText(turnCount);
    }

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D regionCollider = Physics2D.OverlapPoint(worldPosition);
        Region hoveredRegion = regionCollider != null ? regionCollider.GetComponent<Region>() : null;

        if (hoveredRegion != highlightedRegion)
        {
            UpdateRegion(hoveredRegion);
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (hoveredRegion != null && selectedArmy == null && selectedCity == null)
                campaignUI.showRegionDetails(hoveredRegion);
            else
                campaignUI.hideRegionDetails();
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            campaignUI.hideRegionDetails(); //should never show details after a left click
        }
    }

    /// <summary>
    /// updates the currently cached "highlighted region" inside of the CampaignMapManager
    /// </summary>
    /// <param name="region"></param>
    public void UpdateRegion(Region region)
    {
        highlightedRegion = region;
        campaignUI.updateHighlightedRegionUI(region, MapmodeManager.i.mapmode);
    }

    /// <summary>
    /// sets up a campaign, passing the factionCode of the player faction.
    /// </summary>
    /// <param name="playerFaction"></param>
    public void SetupCampaign(string playerFaction)
    {
        i = this;

        //initializing various lists
        factions = new List<MapFaction>();
        mapRegions = new Dictionary<string, Region>();
        factionDictionary = new Dictionary<string, MapFaction>();

        fieldArmies = new List<FieldArmy>();
        cities = new List<MapCity>();

        foreach (var region in regions)
        {
            region.InitializeRegion();
            mapRegions.Add(region.RegionCode, region);
        }

        foreach (var faction in factionBases)
        {
            MapFaction mFaction = new MapFaction(faction); //construct a new faction from its base

            //check if this faction is the player faction
            if (playerFaction.Equals(faction.FactionTag))
            {
                this.playerFaction = mFaction; //store it as the playerFaction in memory
            }

            factions.Add(mFaction); //add it to the factions list
            factionDictionary.Add(mFaction.fBase.FactionTag, mFaction);//also add it to the faction dictionary

            //now assign owned regions to the faction
            foreach (string region in mFaction.fBase.StartingRegions)
            {
                mFaction.OccupyRegion(mapRegions[region]); //update using the dictionary
            }
        }
    }

    /// <summary>
    /// Dictionary that returns a Region from its regionCode.
    /// </summary>
    private Dictionary<string, Region> mapRegions;

    /// <summary>
    /// Dictionary that returns a MapFaction from its factionCode. 
    /// </summary>
    private Dictionary<string, MapFaction> factionDictionary;
}