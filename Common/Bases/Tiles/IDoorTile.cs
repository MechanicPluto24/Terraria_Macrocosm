using Macrocosm.Common.TileFrame;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace Macrocosm.Common.Bases.Tiles;

/// <summary>
/// TML: Maybe add functionality to tModLoader 
/// </summary>
public interface IDoorTile
{
    /// <summary> Door height in tiles. Should be >= 1 </summary>
    public int Height { get; }

    /// <summary> 
    /// Door width in tiles.
    /// For the current implementation, it should be 1 for closed doors; 1 or 2 for open doors. 
    /// </summary>
    public int Width { get; }

    /// <summary> Whether this tile is the closed state of the door </summary>
    public bool IsClosed { get; }

    /// <summary> Whether this door is locked <b>(TODO)</b></summary>
    public bool IsLocked => false;

    /// <summary>
    /// Number of styles for this door. 
    /// For closed doors, it means the number of alternate styles.
    /// For open doors, it should be 1 for sliding doors, or 2 for hinged doors.
    /// </summary>
    public int StyleCount { get; }

    /// <summary> The door activate sound. Defaults to <see cref="SoundID.DoorOpen"/> or <see cref="SoundID.DoorClosed"/>, depending on <see cref="IsClosed"/>. </summary>
    public SoundStyle? ActivateSound => null;

    public AnimationData? AnimationData => null;

    public Rectangle ModifyAutoDoorPlayerCollisionRectangle(Point tileCoords, Rectangle original)
    {
        return original;
    }
}
