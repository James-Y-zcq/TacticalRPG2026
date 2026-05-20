using UnityEngine;

/// <summary>
/// A MapObject is any object that sits on top of a tile. When a tile is clicked, it highlights the current mapobject, shows its object info, etc.
/// </summary>
public interface MapObject
{
    /// <summary>
    /// When the tile is clicked, this function is called for the mapobject
    /// </summary>
    public void onLeftClick();
    
    /// <summary>
    /// Exposes data for the relevant popup menu formatted depending on the type of object (destructible wall, character, etc.). Also outputs a sprite for a windowed view UI
    /// </summary>
    /// <returns></returns>
    public string exposeObjectInfo(out Sprite windowSprite, out string description);
}