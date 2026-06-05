/// <summary>
/// A (abstract) city building is stored inside of a city, and makes up the blocks of a city's infrastructure and recuitment capabilities.
/// </summary>
public abstract class CityBuilding
{
    /// <summary>
    /// Upfront cost to constructing the building
    /// </summary>
    public int constructionCost {get; private set;}

    /// <summary>
    /// How many turns construction takes
    /// </summary>
    public int constructionTime {get; private set;}
    
    /// <summary>
    /// Money cost per turn, deducted during the end turn balance changes
    /// </summary>
    public int buildingMaintenance;

    /// <summary>
    /// Called at the end of a turn to see if it has some passive impact. Used for population growth, etc.
    /// </summary>
    public abstract void endTurnImpact();

    /// <summary>
    /// called on a building when checking what it unlocks.
    /// </summary>
    public abstract void checkBuildingUnlocks();
}