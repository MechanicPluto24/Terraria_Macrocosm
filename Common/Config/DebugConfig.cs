using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace Macrocosm.Common.Config;

public class DebugConfig : ModConfig
{
    /// <summary> This config's instance, assigned by tML before Macrocosm content loads! </summary>
    public static DebugConfig Instance;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(false)]
    public bool RocketBounds { get; set; }

    [DefaultValue(false)]
    public bool RocketIndex { get; set; }

    [DefaultValue(false)]
    public bool RocketCount { get; set; }

    [DefaultValue(false)]
    public bool UIDebug { get; set; }

    [DefaultValue(false)]
    public bool PacketDebug { get; set; }

    [DefaultValue(false)]
    public bool RoomOxygenDebug { get; set; }

    [DefaultValue(false)]
    public bool ShowCursorTileCoords { get; set; }

    [DefaultValue(false)]
    public bool ShowCursorWorldCoords { get; set; }

    /// <summary> Things can subscribe to this event for notification when the configuration has been changed </summary>
    public event EventHandler OnConfigChanged;
    public override void OnChanged() => OnConfigChanged?.Invoke(this, EventArgs.Empty);
}
