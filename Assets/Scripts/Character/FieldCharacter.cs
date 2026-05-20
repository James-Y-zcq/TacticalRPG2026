using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The graphical frontend of a combat unit.
/// </summary>
public class FieldCharacter : MonoBehaviour, ObjectHP
{
    #region Fields
    [SerializeField] OverlayTile tilePosition;
    [SerializeField] OverlayTile targetTile;
    [SerializeField] List<Vector2> movementOrders;
    public string internalCharacterName { get; private set; }

    [SerializeField] CharacterSpriteHandler spriteHandler;
    public CharacterSpriteHandler SpriteHandler => spriteHandler;

    [SerializeField] UnitBase defaultBase;
    private Unit unit;
    
    private static Vector2 positionalOffset = new Vector2(0, 0.25f); //change to make it so character positions reflect properly on the isometric grid
    #endregion
    #region Setup
    void Start()
    {
        SetupUnit(defaultBase, 4 * Vector2.one);
    }
    public void SetupUnit(UnitBase uBase, Vector2 tilePosition)
    {
        unit = new Unit(uBase);
        internalCharacterName = uBase.UnitName; //bubbled up 
        spriteHandler.Setup(uBase.CombatSprites);

        SetTilePosition(tilePosition);
    }
    #endregion
    #region Tile Movement
    public void SetTilePosition(Vector2 tilePosition)
    {
        // null check for map manager
        if (MapManager.i == null)
        {
            Debug.LogError("MapManager instance not found when setting up field character.");
            return;
        }

        //check to see if a tile exists at this position
        if (!MapManager.i.TryGetOverlayTile(tilePosition, out OverlayTile currentTile))
        {
            Debug.LogError($"No overlay tile found at tile position {tilePosition}.");
            return;
        }

        this.tilePosition = currentTile;
        targetTile = currentTile;
        
        if(currentTile != null)
            currentTile.SetRestingObject(this); //set the current tile as its resting object
        
        transform.position = (Vector2)currentTile.transform.position + positionalOffset;
    }

    private float moveDuration = 0.15f;
    public IEnumerator FollowPath(){ //coroutine to follow a path
        if (movementOrders == null || movementOrders.Count == 0)
        {
            yield break; //early termination
        }

        foreach (var point in movementOrders)
        {
            if (!MapManager.i.TryGetOverlayTile(point, out OverlayTile nextTile))
            {
                Debug.LogError($"No overlay tile found at path position {point}.");
                continue;
            }

            targetTile = nextTile;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = nextTile.transform.position + (Vector3)positionalOffset;
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration) //the actual lerp
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            tilePosition = nextTile;
        }

        movementOrders.Clear();
    }
    #endregion
    #region MapObject and HP implementation
    public void onLeftClick()
    {
        //play a sound effect for clicking on a party member based on its party affiliation
    }
    public string exposeObjectInfo(out Sprite windowSprite, out string description)
    {
        windowSprite = unit._base.PortraitSprite;
        description = unit._base.UnitDescription;
        return unit._base.UnitName;
    }
    public IEnumerator TakeDamage(int taken)
    {
        throw new NotImplementedException();
    }
    #endregion
}