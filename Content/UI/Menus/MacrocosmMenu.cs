using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.UI.Menus
{
	public class MacrocosmMenu : ModMenu
    {
        private const string AssetPath = "Macrocosm/Content/UI/Menus/";
		private const AssetRequestMode Mode = AssetRequestMode.ImmediateLoad;

        private readonly List<CelestialBody> celestialBodies = new();

        public readonly StarsDrawing Stars = new();

        public readonly CelestialBody Sun = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Sun", Mode).Value);

        public readonly CelestialBody Vulcan = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Vulcan", Mode).Value);
        public readonly CelestialBody Mercury = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Mercury", Mode).Value);
        public readonly CelestialBody Venus = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Venus", Mode).Value);
        public readonly CelestialBody Earth = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Earth", Mode).Value);
        public readonly CelestialBody Luna = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Luna", Mode).Value);
        public readonly CelestialBody Mars = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Mars", Mode).Value);
        public readonly CelestialBody Phobos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Phobos", Mode).Value);
        public readonly CelestialBody Deimos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Deimos", Mode).Value);
        public readonly CelestialBody Ceres = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Ceres", Mode).Value);
        public readonly CelestialBody Jupiter = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Jupiter", Mode).Value);
        public readonly CelestialBody Io = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Io", Mode).Value);
        public readonly CelestialBody Europa = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Europa", Mode).Value);
        public readonly CelestialBody Saturn = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Saturn", Mode).Value,
													ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SaturnRings", Mode).Value);
		public readonly CelestialBody Titan = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Titan", Mode).Value);
        public readonly CelestialBody Ouranos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Ouranos", Mode).Value,
													ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/OuranosRings", Mode).Value);
        public readonly CelestialBody Miranda = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Miranda", Mode).Value);
        public readonly CelestialBody Neptune = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Neptune", Mode).Value);
        public readonly CelestialBody Triton = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Triton", Mode).Value);
        public readonly CelestialBody Pluto = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Pluto", Mode).Value);
        public readonly CelestialBody Charon = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Charon", Mode).Value);
        public readonly CelestialBody Eris = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Eris", Mode).Value);

        public MacrocosmMenu()
        {
            celestialBodies = new()
            {
				//Sun,
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
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>(AssetPath + "Logo");

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

        private SpriteBatchState state1, state2;
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Rectangle screen = new(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screen, Color.Black);
			state1.SaveState(spriteBatch);
            spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state1);

			Stars.Draw(spriteBatch);
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetPath + "MilkyWay").Value, screen, Color.White.NewAlpha(0.3f)); 
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetPath + "Nebula").Value, screen, Color.White.NewAlpha(0.75f));

			spriteBatch.End();
			spriteBatch.Begin(state1);

            Sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);

            DrawSunCorona();

			Sun.OverrideShader = () => {
				Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "RadialSaturation", AssetRequestMode.ImmediateLoad).Value;
				effect.Parameters["uCenter"].SetValue(Vector2.One * 0.5f);
				effect.Parameters["uRadius"].SetValue(0.1f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 2));
				effect.Parameters["uIntensity"].SetValue(0.2f * Utility.PositiveSineWave(500, MathF.PI / 2));
				return effect;
			};

			logoScale *= 0.65f;
            drawColor = Color.White;

			Sun.Scale = 0.85f + 0.01f * Utility.SineWave(500, MathF.PI / 2);

			// includes all orbiting descendants in the tree 
			Sun.Draw(spriteBatch, withChildren: true);

            return true;
        }

        private void SetupCelestialBodies()
        {
            foreach (CelestialBody body in celestialBodies)
            {
                body.Scale = 0.6f;

                body.SetLightSource(Sun);
                body.ConfigureShader = (float rotation, out float intensity, out Vector2 offset) =>
                {
                    intensity = 0.95f;
                    offset = Utility.PolarVector(0.2f, rotation);
                };
            }

            Sun.Position = Utility.ScreenCenter;
         
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


		private static float Rand() => Utility.RandomRotation();

        private void DrawSunCorona()
        {
			Texture2D SunCorona1 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona1", Mode).Value;
            Texture2D SunCorona2 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona2", Mode).Value;
            Texture2D SunCorona3 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona3", Mode).Value;
            Texture2D SunCorona4 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona4", Mode).Value;
            Texture2D SunCorona5 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona5", Mode).Value;

            SpriteBatch spriteBatch = Main.spriteBatch;

			state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, state2);

			spriteBatch.Draw(SunCorona1, Sun.Position, null, (Color.White * (0.4f + 0.8f * Utility.PositiveSineWave(800, 0f          ))).NewAlpha(0.6f + 0.2f * Utility.PositiveSineWave(800, 0f          )), 0, SunCorona1.Size() / 2, 0.85f         + (0.04f * Utility.SineWave(800, 0f        )), SpriteEffects.None, 0f);
            spriteBatch.Draw(SunCorona2, Sun.Position, null, (Color.White * (0.6f + 0.4f * Utility.PositiveSineWave(600, MathF.PI / 8))).NewAlpha(0.6f + 0.4f * Utility.PositiveSineWave(600, MathF.PI / 8)), 0, SunCorona1.Size() / 2, 0.85f         + (0.03f * Utility.SineWave(600, MathF.PI/8)), SpriteEffects.None, 0f);
            spriteBatch.Draw(SunCorona3, Sun.Position, null, (Color.White * (0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 4))).NewAlpha(0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 4)), 0, SunCorona1.Size() / 2, 0.85f         + (0.03f * Utility.SineWave(500, MathF.PI/3)), SpriteEffects.None, 0f);
            spriteBatch.Draw(SunCorona4, Sun.Position, null, (Color.White * (0.7f + 0.3f * Utility.PositiveSineWave(500, MathF.PI / 2))).NewAlpha(0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 2)), 0, SunCorona1.Size() / 2, 0.85f         + (0.02f * Utility.SineWave(500, MathF.PI/2)), SpriteEffects.None, 0f);
            spriteBatch.Draw(SunCorona5, Sun.Position, null, (Color.White * (0.6f + 0.4f * Utility.PositiveSineWave(300, MathF.PI / 2))).NewAlpha(0.9f + 0.1f * Utility.PositiveSineWave(300, MathF.PI / 2)), 0, SunCorona1.Size() / 2, 0.85f * 0.95f + (0.02f * Utility.SineWave(300, MathF.PI/2)), SpriteEffects.None, 0f);
       
            spriteBatch.End();
            spriteBatch.Begin(state2);
        }
    }
}
