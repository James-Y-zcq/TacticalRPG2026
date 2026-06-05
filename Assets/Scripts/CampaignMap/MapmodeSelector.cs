using System.Collections.Generic;
using UnityEngine;

public class MapmodeSelector : MonoBehaviour
{
    [SerializeField] List<MapmodeButton> mapmodeButtons;
    void OnEnable()
    {
        foreach (var button in mapmodeButtons)
        {
            if (button != null)
            {
                button.onMapmodeSelected += onMapmodeButtonPressed;
            }
        }
    }
    void OnDisable()
    {
        foreach (var button in mapmodeButtons)
        {
            if (button != null)
            {
                button.onMapmodeSelected -= onMapmodeButtonPressed;
            }
        }
    }

    private void onMapmodeButtonPressed(Mapmode mapmode)
    {
        MapmodeManager.i.updateMapmode(mapmode);
    }
}
