using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace Macrocosm.Common.Config
{
    /// <summary> Config class for miscellaneous configuration, client-side only </summary>
    public class ClientConfig : ModConfig
    {
        /// <summary> This config's instance, assigned by tML before Macrocosm content loads! </summary>
        public static ClientConfig Instance;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        /// <summary> Things can subscribe to this event for notification when the configuration has been changed </summary>
        public event EventHandler OnConfigChanged;

        [Header("$Mods.Macrocosm.Configs.ClientConfig.GameplayHeader")]
        [DefaultValue(false)]
        public bool AlwaysDisplayTitleCards { get; set; }

        [DefaultValue(true)]
        public bool GunRecoilEffects { get; set; }

        [Header("$Mods.Macrocosm.Configs.ClientConfig.UIHeader")]
        [DrawTicks]
        [DefaultValue("Macrocosm")]
        [OptionStrings(["Macrocosm", "Terraria"])]
        public string SelectedUITheme { get; set; }

        /// <summary> Supported unit systems in Macrocosm </summary>
        public enum UnitSystemType { Metric, Imperial };

        /// <summary> The unit system used for displays throughout the mod </summary>
        [DefaultValue(UnitSystemType.Metric)]
        public UnitSystemType UnitSystem { get; set; }

        /// <summary> Whether to display gravity in Gs or as gravtiational acceleration </summary>
        [DefaultValue(true)]
        public bool DisplayGravityInGs { get; set; }

        /*
        [Header("$Mods.Macrocosm.Configs.ClientConfig.GraphicsHeader")]
        [DefaultValue(false)]
        public bool HighResolutionEffects { get; set; }
        */
        public override void OnChanged()
        {
            OnConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
