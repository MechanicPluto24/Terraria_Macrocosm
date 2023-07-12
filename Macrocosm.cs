using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Tiles.Blocks;
using Terraria.GameContent;

namespace Macrocosm
{
    public class Macrocosm : Mod
	{
		public static Mod Instance => ModContent.GetInstance<Macrocosm>();

		public const string EffectAssetsPath = "Macrocosm/Assets/Effects/";
		public const string TextureAssetsPath = "Macrocosm/Assets/Textures/";
		public const string MusicAssetsPath = "Macrocosm/Assets/Music/";
		public const string SFXAssetsPath = "Macrocosm/Assets/SFX/";

		public const string EmptyTexPath = TextureAssetsPath + "Empty";
		public static Asset<Texture2D> EmptyTexAsset => ModContent.Request<Texture2D>(EmptyTexPath);
		public static Texture2D EmptyTex => EmptyTexAsset.Value;

		public override void Load()
		{
			LoadModCalls();
			LoadEffects();
			ApplyResprites();
		}

		private static void LoadEffects()
		{
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
			
			Filters.Scene["Macrocosm:RadiationNoise"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>(EffectAssetsPath + "RadiationNoise", mode).Value), "RadiationNoise"));
			Filters.Scene["Macrocosm:RadiationNoise"].Load();
		}

		private void LoadModCalls()
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

		private void ApplyResprites()
		{
			string respritePath = Macrocosm.TextureAssetsPath + "Resprites/";
			TextureAssets.Moon[0] = ModContent.Request<Texture2D>(respritePath + "Moon_0");
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			PacketHandler.HandlePacket(reader, whoAmI);
		}
	}
}