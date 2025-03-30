using Macrocosm.Common.Enums;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace Macrocosm.Common.Config
{
    /// <summary> Config class for miscellaneous configuration, client-side only </summary>
    public class ServerConfig : ModConfig
    {
        /// <summary> This config's instance, assigned by tML before Macrocosm content loads! </summary>
        public static ServerConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("WorldGenHeader")]
        [DefaultValue(false)]
        public bool DisableSilicaSandGeneration { get; set; }

        [DefaultValue(false)]
        public bool DisableOilShaleGeneration { get; set; }

        [Header("UpdatesHeader")]
        [DefaultValue(UpdateRate.Realtime)]
        public UpdateRate CircuitSearchUpdateRate { get; set; }

        [DefaultValue(UpdateRate.Realtime)]
        public UpdateRate CircuitSolveUpdateRate { get; set; }

        [DefaultValue(UpdateRate.Realtime)]
        public UpdateRate ClusterFindUpdateRate { get; set; }

        [DefaultValue(UpdateRate.Realtime)]
        public UpdateRate RoomPressureUpdateRate { get; set; }

        /// <summary> Things can subscribe to this event for notification when the configuration has been changed </summary>
        public event EventHandler OnConfigChanged;
        public override void OnChanged() => OnConfigChanged?.Invoke(this, EventArgs.Empty);
    }
}
