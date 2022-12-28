using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Menu
{
    public class MacrocosmMenu : ModMenu
	{

		private const string assetPath = "Macrocosm/Content/Menus/";
		private readonly List<CelestialBody> celestialBodies = new();

		public readonly StarsDrawing Stars = new();

		public readonly CelestialBody Sun = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Sun", AssetRequestMode.ImmediateLoad).Value,
												 ModContent.Request<Texture2D>(assetPath + "CelestialBodies/SunCorona", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Vulcan = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Vulcan", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Mercury = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Mercury", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Venus = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Venus", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Earth = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Earth", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Luna = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Luna", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Mars = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Mars", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Phobos = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Phobos", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Deimos = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Deimos", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Ceres = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Ceres", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Jupiter = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Jupiter", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Io = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Io", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Europa = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Europa", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Saturn = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Saturn", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Titan = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Titan", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Ouranos = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Ouranos", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Miranda = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Miranda", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Neptune = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Neptune", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Triton = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Triton", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Pluto = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Pluto", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Charon = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Charon", AssetRequestMode.ImmediateLoad).Value);
		public readonly CelestialBody Eris = new(ModContent.Request<Texture2D>(assetPath + "CelestialBodies/Eris", AssetRequestMode.ImmediateLoad).Value);

		public MacrocosmMenu()
		{
			celestialBodies = new()
			{
				Sun,
 				Vulcan,
				Mercury,
				Venus,
				Earth,
				Luna,
				Mars,
				Phobos,
				Deimos,
				Ceres,
				Jupiter,
				Io,
				Europa,
				Saturn,
				Titan,
				Ouranos,
				Miranda,
				Neptune,
				Triton,
				Pluto,
				Charon,
				Eris
			};
		}
		public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>(assetPath + "Logo");

		public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>(Macrocosm.EmptyTexPath);

		public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(Macrocosm.EmptyTexPath);

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld");

		public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;

		public override string DisplayName => "Macrocosm";

		public override void OnSelected()
		{
			Stars.SpawnStars(350, 500, 0.8f);
			SetupCelestialBodies();
		}

		public override void OnDeselected()
		{
			Stars.Clear();
			Sun.ClearOrbitChildren();
		}

		public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
		{
			Rectangle screen = new(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
			spriteBatch.Draw(TextureAssets.BlackTile.Value, screen, Color.Black);

			SpriteBatchState state = spriteBatch.SaveState();
			spriteBatch.EndIfBeginCalled();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state);

			spriteBatch.Draw(ModContent.Request<Texture2D>(assetPath + "MilkyWay").Value, screen, Color.White.NewAlpha(0.6f));
			spriteBatch.Draw(ModContent.Request<Texture2D>(assetPath + "Nebula").Value, screen, Color.White.NewAlpha(0.8f));

			spriteBatch.Restore(state);

			Stars.Draw(spriteBatch);

			// includes all orbiting descendants in the tree 
			Sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);
			Sun.Draw(spriteBatch);

			logoScale *= 0.65f;
			drawColor = Color.White;

			return true;
		}

		private void SetupCelestialBodies()
		{
			foreach (CelestialBody body in celestialBodies)
				body.Scale = 0.6f;

			Sun.Scale = 0.85f;

			Sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);
			Vulcan.SetOrbitParent(Sun, 174, Rand(), 0.0022f);
			Mercury.SetOrbitParent(Sun, 204, Rand(), 0.0018f);
			Venus.SetOrbitParent(Sun, 238, Rand(), 0.0014f);

			Earth.SetOrbitParent(Sun, 288, Rand(), 0.001f);
			Luna.SetOrbitParent(Earth, new Vector2(36, 10), 0f, Rand(), 0.018f);

			Mars.SetOrbitParent(Sun, 330, Rand(), 0.0008f);
			Phobos.SetOrbitParent(Mars, new Vector2(20, 8), 0.2f, Rand(), 0.014f);
			Deimos.SetOrbitParent(Mars, new Vector2(24, 10), -0.4f, Rand(), 0.016f);

			Ceres.SetOrbitParent(Sun, 362, Rand(), 0.0006f);

			Jupiter.SetOrbitParent(Sun, 410, Rand(), 0.0004f);
			Io.SetOrbitParent(Jupiter, new Vector2(48, 20), 0.2f, Rand(), 0.012f);
			Europa.SetOrbitParent(Jupiter, new Vector2(54, 18), 0.1f, Rand(), 0.01f);

			Saturn.SetOrbitParent(Sun, 514, Rand(), 0.00037f);
			Titan.SetOrbitParent(Saturn, new Vector2(52, 28), 0.8f, Rand(), 0.012f);

			Ouranos.SetOrbitParent(Sun, 622, Rand(), 0.0003f);
			Miranda.SetOrbitParent(Ouranos, new Vector2(42, 18), 0f, Rand(), 0.017f);

			Neptune.SetOrbitParent(Sun, 700, Rand(), 0.00027f);
			Triton.SetOrbitParent(Neptune, new Vector2(36, 26), 0.9f, Rand(), 0.018f);

			CelestialBody plutoBarycenter = new();
			plutoBarycenter.SetOrbitParent(Sun, new Vector2(760, 620), 0.28f, Rand(), 0.00022f);
			plutoBarycenter.AddOrbitChild(Pluto, 4, 0f, 0.005f);
			Pluto.AddOrbitChild(Charon, 18, 3.14f, 0.008f);

			Eris.SetOrbitParent(Sun, 810, Rand(), 0.00018f);
		}

		private static float Rand() => MathUtils.RandomRotation();
	}
}
