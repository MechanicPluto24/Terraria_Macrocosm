namespace Macrocosm.Common.Storage;

public enum InventorySlotRole
{
    /// <summary> Player and automation may insert and extract. </summary>
    General,

    /// <summary> Player and automation may insert; only the player and internal logic may extract. </summary>
    Input,

    /// <summary> Only internal logic may insert; player and automation may extract. </summary>
    Output,

    /// <summary> Only internal logic may insert; only the player and internal logic may extract. </summary>
    OutputLocked
}
