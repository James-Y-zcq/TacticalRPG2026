using TMPro;
using UnityEngine;

/// <summary>
/// Stores functions pertaining to campaign UI.
/// </summary>
public class CampaignUI : MonoBehaviour
{
    #region UI elements
    [SerializeField] TMP_Text regionText;
    [SerializeField] RegionDetailsMenu regionDetails;
    #endregion

    /// <summary>
    /// updates the highlighted region UI. Content changes based on the current mapmode.
    /// </summary>
    /// <param name="region"></param>
    /// <param name="currentMode"></param>
    public void updateHighlightedRegionUI(Region region, Mapmode currentMode)
    {
        if(currentMode == Mapmode.political)
            regionText.text = region != null ? $"{region.RegionName}" : string.Empty;
        else if(currentMode == Mapmode.terrain)
            regionText.text = region != null ? $"{region.RegionName} : {region.RegionalTerrain.TerrainName}" : string.Empty;
    }

    public void showRegionDetails(Region region)
    {
        regionDetails.enableDetailsGraphics(true);

        regionDetails.showRegionDetails(region.RegionName, region.LocalNoble.NobleName, region.currentPopulation, region.RegionalBanner);
    }

    public void hideRegionDetails()
    {
        regionDetails.enableDetailsGraphics(false);
    }
}