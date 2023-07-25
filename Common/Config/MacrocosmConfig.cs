using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Macrocosm.Common.Config
{
	/// <summary> Config class for miscellaneous configuration, client-side only </summary>
	public class MacrocosmConfig : ModConfig
	{
		/// <summary> This config's instance, assigned by tML before Macrocosm content loads! </summary>
		public static MacrocosmConfig Instance;

		/// <summary> Things can subscribe to this event for notification when the configuration has been changed </summary>
		public event EventHandler OnConfigChanged;

		public override ConfigScope Mode => ConfigScope.ClientSide;

		/// <summary> Supported unit systems in Macrocosm </summary>
		public enum UnitSystemType { Metric, Imperial };

		/// <summary> The unit system used for displays throughout the mod </summary>
		[Header("$Mods.Macrocosm.Configs.MacrocosmConfig.UnitSystemHeader")]
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
