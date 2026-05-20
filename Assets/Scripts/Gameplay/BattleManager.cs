using UnityEngine;

/// <summary>
/// Main controller for the flow of combat states. Make sure to outsource as much functionality as possible to prevent a script 2000+ lines long.
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager i;
    void Awake()
    {
        if(i==null) i = this;
    }
    
    void Update()
    {
        
    }
}

public enum BattleState
{
    SelectPartyMember,
    SelectAction,
    SelectTarget
}