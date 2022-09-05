﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Common.Drawing;
using Terraria.UI.Chat;
using Macrocosm.Common.Utility;

namespace Macrocosm.Content
{
	public class MacrocosmMenu : ModMenu
	{

		private const string assetPath = "Macrocosm/Assets/Textures/Menu/";

		private readonly StarsDrawing stars = new();
		private readonly List<CelestialBody> celestialBodies = new();

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
			celestialBodies.Add(Sun	);
			celestialBodies.Add(Vulcan);
			celestialBodies.Add(Mercury);
			celestialBodies.Add(Venus);
			celestialBodies.Add(Earth);
			celestialBodies.Add(Luna);
			celestialBodies.Add(Mars);
			celestialBodies.Add(Phobos);
			celestialBodies.Add(Deimos);
			celestialBodies.Add(Ceres);
			celestialBodies.Add(Jupiter);
			celestialBodies.Add(Io);
			celestialBodies.Add(Europa);
			celestialBodies.Add(Saturn);
			celestialBodies.Add(Titan);
			celestialBodies.Add(Ouranos);
			celestialBodies.Add(Miranda);
			celestialBodies.Add(Neptune);
			celestialBodies.Add(Triton);
			celestialBodies.Add(Pluto);
			celestialBodies.Add(Charon);
			celestialBodies.Add(Eris);
		}

		public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>(assetPath + "Logo");

		public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Empty");

		public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/Empty");

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld");

		public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;

		public override string DisplayName => "Macrocosm";

		public override void OnSelected()
		{
			stars.SpawnStars(350, 500, 0.8f);
			SetupCelestialBodies();
 		}

		public override void OnDeselected()
		{
			stars.Clear();
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

			stars.Draw(spriteBatch);

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
			Sun.ResetOrbitChildren();

			Sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);
			Vulcan.SetOrbitParent(Sun, 174, Rand(), 0.0022f);
			Mercury.SetOrbitParent(Sun, 204, Rand(), 0.0018f);
			Venus.SetOrbitParent(Sun, 238, Rand(), 0.0014f);

			Earth.SetOrbitParent(Sun, 288, Rand(), 0.001f);
			Luna.SetOrbitParent(Earth, 36, Rand(), 0.02f);

			Mars.SetOrbitParent(Sun, 330, Rand(), 0.0008f);
			Phobos.SetOrbitParent(Mars, 20, Rand(), 0.014f);
			Deimos.SetOrbitParent(Mars, 24, Rand(), 0.016f);

			Ceres.SetOrbitParent(Sun, 362, Rand(), 0.0006f);

			Jupiter.SetOrbitParent(Sun, 410, Rand(), 0.0004f);
			Io.SetOrbitParent(Jupiter, 48, Rand(), 0.012f);
			Europa.SetOrbitParent(Jupiter, 56, Rand(), 0.01f);

			Saturn.SetOrbitParent(Sun, 514, Rand(), 0.00037f);
			Titan.SetOrbitParent(Saturn, 40, Rand(), 0.012f);

			Ouranos.SetOrbitParent(Sun, 622, Rand(), 0.0003f);
			Miranda.SetOrbitParent(Ouranos, 42, Rand(), 0.017f);

			Neptune.SetOrbitParent(Sun, 700, Rand(), 0.00027f);
			Triton.SetOrbitParent(Neptune, 36, Rand(), 0.018f);

			Pluto.SetOrbitParent(Sun, 760, Rand(), 0.00022f);
			Charon.SetOrbitParent(Pluto, 14, Rand(), 0.021f);

			Eris.SetOrbitParent(Sun, 810, Rand(), 0.00018f);
 		}

		private static float Rand() => MiscUtils.RandomRotation();
	}
}