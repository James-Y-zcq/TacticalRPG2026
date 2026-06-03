using UnityEngine;

public class MapmodeManager : MonoBehaviour
{
    public mapmode mapmode {get; private set;}

    /// <summary>
    /// branching if-statement that handles a varied mapmode.
    /// </summary>
    /// <param name="mode"></param>
    public void updateMapmode(mapmode mode)
    {
        this.mapmode = mode;

        if(this.mapmode == mapmode.political)
        {
            
        }
        else if(mapmode == mapmode.terrain)
        {
            
        }
    }
}

public enum mapmode
{
    political,
    terrain
}