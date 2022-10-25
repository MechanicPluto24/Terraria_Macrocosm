using Macrocosm.Common;
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

		public const string EmptyTexPath = "Macrocosm/Assets/Textures/Empty";
		public static Texture2D EmptyTex { get; set; }

		public override void Load()
		{
			EmptyTex = ModContent.Request<Texture2D>(EmptyTexPath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			EffectLoader.LoadEffects();

			#region Ryan's mods calls
			if (ModLoader.TryGetMod("TerrariaAmbience", out Mod ta))
				ta.Call("AddTilesToList", null, "Stone", Array.Empty<string>(), new int[] 
				{ 
					ModContent.TileType<Regolith>(),
					ModContent.TileType<Protolith>()
				});

			if (ModLoader.TryGetMod("TerrariaAmbienceAPI", out Mod taAPI))
				taAPI.Call("Ambience", this, "MoonAmbience", "Assets/Sounds/Ambient/Moon", 1f, 0.0075f, new Func<bool>(SubworldSystem.IsActive<Moon>));
			#endregion
		}

	}
}