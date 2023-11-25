using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Macrocosm.Common.Config
{
    /// <summary> Config class for miscellaneous configuration, client-side only </summary>
    public class MacrocosmConfig : ModConfig
    {
        /// <summary> This config's instance, assigned by tML before Macrocosm content loads! </summary>
        #pragma warning disable CA2211 // Non-constant fields should not be visible
        public static MacrocosmConfig Instance;
        #pragma warning restore CA2211

        /// <summary> Things can subscribe to this event for notification when the configuration has been changed </summary>
        public event EventHandler OnConfigChanged;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.Macrocosm.Configs.MacrocosmConfig.GameplayHeader")]
        [DefaultValue(false)]
        public bool AlwaysDisplayTitleScreens { get; set; }

        [DefaultValue(true)]
        [ReloadRequired]
        public bool FancyGunEffects { get; set; }

        [Header("$Mods.Macrocosm.Configs.MacrocosmConfig.UIHeader")]
        [DrawTicks]
        [DefaultValue("Macrocosm")]
        [OptionStrings(new string[] { "Macrocosm", "Terraria" })]
        public string SelectedUITheme { get; set; }

        /// <summary> Supported unit systems in Macrocosm </summary>
        public enum UnitSystemType { Metric, Imperial };

        /// <summary> The unit system used for displays throughout the mod </summary>
        [DefaultValue(UnitSystemType.Metric)]
        public UnitSystemType UnitSystem { get; set; }

        /// <summary> Whether to display gravity in Gs or as gravtiational acceleration </summary>
        [DefaultValue(true)]
        public bool DisplayGravityInGs { get; set; }

        public override void OnChanged()
        {
            OnConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
