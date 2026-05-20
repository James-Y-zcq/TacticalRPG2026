using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    #region fields
    public static MouseController i;
    private PathFinder pathFinder;

    private OverlayTile cachedTile;
    public event Action<BattleState> updateBattleState;
    #endregion
    
    void Awake()
    {
        i = this;
        pathFinder = new PathFinder();
    }
    public void HandleUpdate(BattleState battleState)
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            cachedTile = overlayTile.GetComponent<OverlayTile>(); //assign the cached tile test
            transform.position = overlayTile.transform.position;

            if(cachedTile != null)
            {
                HandleFocusedTile(battleState);
            }
        }
    }
    private void HandleFocusedTile(BattleState battleState)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            cachedTile?.ShowTile();

            //see if there is a resting object.
            if (cachedTile.RestingObject != null)
            {
                //if so, make it display in the worldObjectPreviewUI
                WorldObjectPreviewUI.i.displayObject(cachedTile.RestingObject);

                //if it is a fieldCharacter party-controlled, and the battlestate is SelectPartyMember, select it and update the battlemanager's state.
            }
            else
            {
                WorldObjectPreviewUI.i.hideMenu();
            }
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame && cachedTile != null)
        {
            
        }
    }
    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }
}