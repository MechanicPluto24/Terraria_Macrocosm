using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Common.CrossMod;

/// <summary> Handles general cross mod compatibility. </summary>
internal static class CrossModLoader
{
	public readonly record struct ModEntry(string Name)
	{
		public readonly string name = Name;

		public readonly bool Enabled
		{
			get
			{
				if (LoadedMods.ContainsKey(name))
					return true;

				if (ModLoader.TryGetMod(name, out var mod))
				{
					LoadedMods.Add(name, mod);
					return true;
				}

				return false;
			}
		}

		/// <summary> The mod instance associated with this entry.
		/// <br/>Should not be used unless you know that this mod is enabled (<see cref="Enabled"/>). </summary>
		public readonly Mod Instance
		{
			get
			{
				if (LoadedMods.TryGetValue(name, out var mod))
					return mod;

				var getMod = ModLoader.GetMod(name);
				LoadedMods.Add(name, getMod);

				return getMod;
			}
		}

		/// <inheritdoc cref="Mod.TryFind{T}(string, out T)"/>
		public readonly bool TryFind<T>(string s, out T t) where T : ModType => ((Mod)this).TryFind(s, out t);
		public static explicit operator Mod(ModEntry e) => e.Instance;
	}

	// Add other mods here
	public static readonly ModEntry Redemption = new("Redemption");

	/// <summary> The names and instances of loaded crossmod mods per <see cref="ModEntry"/>. </summary>
	private static readonly Dictionary<string, Mod> LoadedMods = [];
}