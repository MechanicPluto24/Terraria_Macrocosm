namespace Macrocosm.Common.Storage;

public enum InventoryExtractionSource
{
    /// <summary> Machine or inventory-owner logic. </summary>
    Internal,

    /// <summary> Direct or bulk player interaction. </summary>
    Player,

    /// <summary> Pipes, hoppers, and droppers. </summary>
    Automation
}
