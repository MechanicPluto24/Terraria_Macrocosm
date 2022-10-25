using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using SubworldLibrary;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;

namespace Macrocosm
{
	public class Macrocosm : Mod
	{
		public static Mod Instance => ModContent.GetInstance<Macrocosm>();

		public const string EmptyTexPath = "Macrocosm/Assets/Textures/Empty";
		public const string EffectAssetPath = "Macrocosm/Assets/Effects/";

		public static Texture2D EmptyTex { get; set; }

		public override void Load()
		{
			EmptyTex = ModContent.Request<Texture2D>(EmptyTexPath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			ModCalls();
			LoadEffects();
		}

		private static void LoadEffects()
		{
			Filters.Scene["Macrocosm:RadiationNoiseEffect"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>(EffectAssetPath + "RadiationNoiseEffect", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "RadiationNoiseEffect"));
			Filters.Scene["Macrocosm:RadiationNoiseEffect"].Load();
		}

		private void ModCalls()
		{
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