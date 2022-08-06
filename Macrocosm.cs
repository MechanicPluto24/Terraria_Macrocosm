using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria.ModLoader;

namespace Macrocosm
{
	public class Macrocosm : Mod
	{
		public static Mod Instance => ModContent.GetInstance<Macrocosm>();

		public const string EmptyTexPath = "Macrocosm/Assets/Empty";
		public static Texture2D EmptyTex { get; set; }
		public static Mod GetModOrNull(string name) => ModLoader.TryGetMod(name, out Mod result) ? result : null;

		public override void Load()
		{

			EmptyTex = ModContent.Request<Texture2D>(EmptyTexPath).Value;

			#region Ryan's mods calls

			Mod ta = GetModOrNull("TerrariaAmbience");
			if (ta == null)
				ta?.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[] { ModContent.TileType<Regolith>() });


			Mod taAPI = GetModOrNull("TerrariaAmbienceAPI");
			if (taAPI != null)
				taAPI?.Call("Ambience", this, "MoonAmbience", "Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));

			#endregion
		}

	}
}