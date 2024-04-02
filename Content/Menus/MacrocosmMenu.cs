using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Menus
{
    public class MacrocosmMenu : ModMenu
    {
        private const string AssetPath = "Macrocosm/Content/Menus/";
        private const AssetRequestMode Mode = AssetRequestMode.ImmediateLoad;

        private readonly List<CelestialBody> interactible = new();
        private readonly List<CelestialBody> celestialBodies = new();

        public readonly Stars Stars = new();

        public readonly CelestialBody Sun = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Sun", Mode));

        public readonly CelestialBody Vulcan = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Vulcan", Mode));
        public readonly CelestialBody Mercury = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Mercury", Mode));
        public readonly CelestialBody Venus = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Venus", Mode));

        public readonly CelestialBody Earth = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Earth", Mode));
        public readonly CelestialBody Luna = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Luna", Mode));

        public readonly CelestialBody Mars = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Mars", Mode));
        public readonly CelestialBody Phobos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Phobos", Mode));
        public readonly CelestialBody Deimos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Deimos", Mode));

        public readonly CelestialBody Ceres = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Ceres", Mode));
        private readonly Asset<Texture2D> asteroids = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Asteroids", Mode);
        public readonly List<CelestialBody> AsteroidBelt = new();

        public readonly CelestialBody Jupiter = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Jupiter", Mode));
        public readonly CelestialBody Io = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Io", Mode));
        public readonly CelestialBody Europa = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Europa", Mode));

        public readonly CelestialBody Saturn = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Saturn", Mode),
                                                    ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SaturnRings", Mode),
                                                    ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SaturnRings", Mode));
        public readonly CelestialBody Titan = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Titan", Mode));

        public readonly CelestialBody Ouranos = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Ouranos", Mode),
                                                    ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/OuranosRings", Mode));
        public readonly CelestialBody Miranda = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Miranda", Mode));

        public readonly CelestialBody Neptune = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Neptune", Mode));
        public readonly CelestialBody Triton = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Triton", Mode));

        private readonly CelestialBody plutoBarycenter = new(size: new Vector2(100, 100));
        public readonly CelestialBody Pluto = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Pluto", Mode));
        public readonly CelestialBody Charon = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Charon", Mode));

        public readonly CelestialBody Eris = new(ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/Eris", Mode));

        private readonly Asset<Texture2D> sunCorona1 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona1", Mode);
        private readonly Asset<Texture2D> sunCorona2 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona2", Mode);
        private readonly Asset<Texture2D> sunCorona3 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona3", Mode);
        private readonly Asset<Texture2D> sunCorona4 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona4", Mode);
        private readonly Asset<Texture2D> sunCorona5 = ModContent.Request<Texture2D>(AssetPath + "CelestialBodies/SunCorona5", Mode);

        private readonly Asset<Texture2D> logo = ModContent.Request<Texture2D>(AssetPath + "Logo", Mode);

        private readonly Asset<Texture2D> milkyWay = ModContent.Request<Texture2D>(AssetPath + "MilkyWay", Mode);
        private readonly Asset<Texture2D> nebula = ModContent.Request<Texture2D>(AssetPath + "Nebula", Mode);

        private readonly Asset<Effect> radialSaturation = ModContent.Request<Effect>(Macrocosm.ShadersPath + "RadialSaturation", Mode);

        public MacrocosmMenu()
        {
            celestialBodies =
            [
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
            ];

            interactible =
            [
                Vulcan,
                Mercury,
                Venus,
                Earth,
                Mars,
                Ceres,
                Jupiter,
                Saturn,
                Ouranos,
                Neptune,
                plutoBarycenter,
                Eris
            ];
        }
        public override Asset<Texture2D> Logo => logo;
        public override Asset<Texture2D> SunTexture => Macrocosm.EmptyTex;
        public override Asset<Texture2D> MoonTexture => Macrocosm.EmptyTex;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;
        public override string DisplayName => "Macrocosm";

        public override void OnSelected()
        {
            Stars.SpawnStars(350, 500, baseScale: 0.8f);
            SetupCelestialBodies();
        }

        public override void OnDeselected()
        {
            Stars.Clear();
            Sun.ClearOrbitChildren();
            AsteroidBelt.Clear();
            released.Clear();
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
            spriteBatch.Draw(milkyWay.Value, screen, Color.White.WithOpacity(0.3f));
            spriteBatch.Draw(nebula.Value, screen, Color.White.WithOpacity(0.75f));

            spriteBatch.End();
            spriteBatch.Begin(state1);

            Sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);

            DrawSunCorona();

            logoScale *= 0.65f;
            drawColor = Color.White;

            Sun.Scale = 0.85f + 0.01f * Utility.SineWave(500, MathF.PI / 2);

            // includes all orbiting descendants in the tree 
            Sun.Draw(spriteBatch, withChildren: true);


            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state1);
            var flare = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "HighRes/Flare3").Value;
            var scorch1 = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "HighRes/Scorch1").Value;
            spriteBatch.Draw(flare, Sun.Position, null, new Color(255, 96, 4) * (0.9f + 0.1f * SolarFlareProgress), 0f, flare.Size()/2f, 2.3f + 0.2f * SolarFlareProgress, SpriteEffects.None, 0);
            spriteBatch.Draw(scorch1, Sun.Position, null, new Color(255, 193, 0) * (0.3f + 0.05f * SolarFlareProgress), 0.25f * Utility.PositiveTriangleWave(2000), scorch1.Size()/2f, 2.5f + 0.015f * SolarFlareProgress, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(state1);
            Interact();

            return true;
        }

        private void SetupCelestialBodies()
        {
            foreach (CelestialBody body in celestialBodies)
            {
                body.ShouldUpdate = true;
                body.Scale = 0.6f;

                body.SetLightSource(Sun);
                body.ConfigureBodyShader = ConfigureBodyShader;
            }

            Sun.Position = Utility.ScreenCenter;
            Sun.OverrideBodyShader = Sun_GetShader;

            Vulcan.SetOrbitParent(Sun, 174, Rand(), 0.0022f);
            Mercury.SetOrbitParent(Sun, 204, Rand(), 0.0018f);
            Venus.SetOrbitParent(Sun, 238, Rand(), 0.0014f);

            Earth.SetOrbitParent(Sun, 288, Rand(), 0.001f);
            Luna.SetOrbitParent(Earth, new Vector2(36, 10), 0f, Rand(), 0.018f);

            Mars.SetOrbitParent(Sun, 330, Rand(), 0.0008f);
            Phobos.SetOrbitParent(Mars, new Vector2(20, 8), 0.2f, Rand(), 0.014f);
            Deimos.SetOrbitParent(Mars, new Vector2(24, 10), -0.4f, Rand(), 0.016f);

            Ceres.SetOrbitParent(Sun, 362, Rand(), 0.0006f);

            for (int i = 0; i < 500; i++)
            {
                Rectangle sourceRect = asteroids.Frame(verticalFrames: 8, frameY: Main.rand.Next(8));
                CelestialBody asteroid = new(asteroids, scale: 0.5f, bodySourceRect: sourceRect);
                asteroid.SetOrbitParent(Sun, 
                    orbitRadius: 400 + Main.rand.NextFloat(150) * MathF.Sin(Rand()), 
                    orbitRotation: Rand(),
                    orbitSpeed: Main.rand.NextFloat(0.00015f, 0.00045f)
                    );
                asteroid.ConfigureBodyShader = ConfigureBodyShader;
                AsteroidBelt.Add(asteroid);
            }

            Jupiter.SetOrbitParent(Sun, 410, Rand(), 0.0004f);
            Io.SetOrbitParent(Jupiter, new Vector2(48, 20), 0.2f, Rand(), 0.012f);
            Europa.SetOrbitParent(Jupiter, new Vector2(54, 18), 0.1f, Rand(), 0.01f);

            Saturn.SetOrbitParent(Sun, 514, Rand(), 0.00037f);
            Saturn.ConfigureBackShader = Saturn_ConfigureRingsShader;
            Saturn.ConfigureFrontShader = Saturn_ConfigureRingsShader;
            Saturn.OverrideBackDraw = Saturn_DrawRings_Back;
            Saturn.OverrideFrontDraw = Saturn_DrawRings_Front;
            Titan.SetOrbitParent(Saturn, new Vector2(52, 28), 0.8f, Rand(), 0.012f);

            Ouranos.SetOrbitParent(Sun, 622, Rand(), 0.0003f);
            Miranda.SetOrbitParent(Ouranos, new Vector2(42, 18), 0f, Rand(), 0.017f);

            Neptune.SetOrbitParent(Sun, 700, Rand(), 0.00027f);
            Triton.SetOrbitParent(Neptune, new Vector2(36, 26), 0.9f, Rand(), 0.018f);

            plutoBarycenter.SetOrbitParent(Sun, new Vector2(760, 620), 0.28f, Rand(), 0.00022f);
            plutoBarycenter.AddOrbitChild(Pluto, 4, 0f, 0.005f);
            Pluto.AddOrbitChild(Charon, 18, 3.14f, 0.008f);

            Eris.SetOrbitParent(Sun, 810, Rand(), 0.00018f);
        }

        private static float Rand() => Utility.RandomRotation();

        private float SolarFlareProgress => Utility.PositiveSineWave(500, MathF.PI / 2);

        private void DrawSunCorona()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            state2.SaveState(spriteBatch);
            spriteBatch.End();

            Effect effect = radialSaturation.Value;
            effect.Parameters["uCenter"].SetValue(Vector2.One * 0.5f);
            effect.Parameters["uRadius"].SetValue(0.075f + 0.75f * SolarFlareProgress);
            effect.Parameters["uIntensity"].SetValue(0.025f + 0.025f * SolarFlareProgress);

            spriteBatch.Begin(BlendState.NonPremultiplied, effect, state2);

            spriteBatch.Draw(sunCorona1.Value, Sun.Position, null, (Color.White * (0.4f + 0.8f * Utility.PositiveSineWave(800, 0f))).WithOpacity(0.6f + 0.2f * Utility.PositiveSineWave(800, 0f)), 0, sunCorona1.Size() / 2, 0.85f + 0.04f * Utility.SineWave(800, 0f), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona2.Value, Sun.Position, null, (Color.White * (0.6f + 0.4f * Utility.PositiveSineWave(600, MathF.PI / 8))).WithOpacity(0.6f + 0.4f * Utility.PositiveSineWave(600, MathF.PI / 8)), 0, sunCorona1.Size() / 2, 0.85f + 0.03f * Utility.SineWave(600, MathF.PI / 8), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona3.Value, Sun.Position, null, (Color.White * (0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 4))).WithOpacity(0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 4)), 0, sunCorona1.Size() / 2, 0.85f + 0.03f * Utility.SineWave(500, MathF.PI / 3), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona4.Value, Sun.Position, null, (Color.White * (0.7f + 0.3f * Utility.PositiveSineWave(500, MathF.PI / 2))).WithOpacity(0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 2)), 0, sunCorona1.Size() / 2, 0.85f + 0.02f * Utility.SineWave(500, MathF.PI / 2), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona5.Value, Sun.Position, null, (Color.White * (0.6f + 0.4f * Utility.PositiveSineWave(300, MathF.PI / 2))).WithOpacity(0.9f + 0.1f * Utility.PositiveSineWave(300, MathF.PI / 2)), 0, sunCorona1.Size() / 2, 0.85f * 0.95f + 0.02f * Utility.SineWave(300, MathF.PI / 2), SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void ConfigureBodyShader(CelestialBody celestialBody, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
        {
            intensity = -1f;
            float rotationProgress = Math.Abs(MathF.Cos((rotation + MathHelper.Pi)));
            float offsetRadius = MathHelper.Lerp(-0.8f, -0.5f, Utility.QuadraticEaseInOut(rotationProgress));
            offset = Utility.PolarVector(offsetRadius, rotation);
            shadeResolution *= 1;
            radius = MathHelper.Lerp(1.5f, 1.1f, Math.Abs(rotationProgress)) - 0.15f * SolarFlareProgress;
        }

        private Effect Sun_GetShader()
        {
            Effect effect = radialSaturation.Value;
            effect.Parameters["uCenter"].SetValue(Vector2.One * 0.5f);
            effect.Parameters["uRadius"].SetValue(0.2f + 0.1f * SolarFlareProgress);
            effect.Parameters["uIntensity"].SetValue(0.2f + 0.1f * SolarFlareProgress);
            return effect;
        }

        private void Saturn_DrawRings_Back(CelestialBody saturn, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            Matrix uiScaleMatrix = state.Matrix;
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-saturn.Position.X, -saturn.Position.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationX(MathHelper.Pi * 0.4f * 1) *                         // Apply X skew 
                Matrix.CreateRotationY(MathHelper.Pi * 0.2f * 1) *                         // Apply Y skew
                Matrix.CreateTranslation(saturn.Position.X, saturn.Position.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(uiScaleMatrix.M11, uiScaleMatrix.M22, 0f);          // Apply UI scale

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, transformationMatrix);

            Rectangle sourceRect = new(0, 0, rings.Width(), rings.Height()/2);
            Vector2 position = saturn.Position;
            spriteBatch.Draw(rings.Value, position, sourceRect, Color.White, 0f, new Vector2(sourceRect.Width / 2, sourceRect.Height), saturn.Scale, default, 0f);
            spriteBatch.End();
        }

        private void Saturn_DrawRings_Front(CelestialBody saturn, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            Matrix uiScaleMatrix = state.Matrix;
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-saturn.Position.X, -saturn.Position.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationX(MathHelper.Pi * 0.4f * 1) *                         // Apply X skew 
                Matrix.CreateRotationY(MathHelper.Pi * 0.2f * 1) *                         // Apply Y skew
                Matrix.CreateTranslation(saturn.Position.X, saturn.Position.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(uiScaleMatrix.M11, uiScaleMatrix.M22, 0f);          // Apply UI scale

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, transformationMatrix);

            Rectangle sourceRect = new(0, rings.Height() / 2, rings.Width(), rings.Height() / 2);
            Vector2 position = saturn.Position + new Vector2(0, 0);
            spriteBatch.Draw(rings.Value, position, sourceRect, Color.White, 0f, new Vector2(rings.Height()/2f, 0), saturn.Scale, default, 0f);
            spriteBatch.End();
        }

        private void Saturn_ConfigureRingsShader(CelestialBody saturn, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
        {
            intensity = -1f;
            float rotationProgress = Math.Abs(MathF.Cos((rotation + MathHelper.Pi)));
            float offsetRadius = MathHelper.Lerp(-0.45f, -0.4f, Utility.QuadraticEaseInOut(rotationProgress));
            offset = Utility.PolarVector(offsetRadius, rotation + MathHelper.Pi/8);
            radius = MathHelper.Lerp(0.35f, 0.4f, Utility.QuadraticEaseInOut(rotationProgress));
        }

        private CelestialBody grabbed;
        private readonly Dictionary<CelestialBody, int> released = new();
        private void Interact()
        {
            Main.alreadyGrabbingSunOrMoon = false;

            if (grabbed is null && !released.Any())
            {
                foreach (var celestialBody in interactible)
                {
                    if(celestialBody.Hitbox.Contains(Main.mouseX, Main.mouseY) && Main.mouseLeft)
                    {
                        grabbed = celestialBody;
                        break;
                    }
                }
            }

            if (grabbed is not null)
            {
                grabbed.ShouldUpdate = false;
                Vector2 mousePosition = Main.MouseScreen;
                Vector2 targetPosition;

                float minDistanceFromSun = 160 + Math.Max(grabbed.Width, grabbed.Height);
                bool forceRelease = false;
                if (Vector2.Distance(mousePosition, Sun.Position) < minDistanceFromSun)
                {
                    targetPosition = Vector2.Lerp(grabbed.Position, Sun.Position + (mousePosition - Sun.Position) * minDistanceFromSun, 0.0005f);

                    if (Vector2.Distance(grabbed.Position, mousePosition) > 80f)
                        forceRelease = true;
                }
                else
                {
                    Main.alreadyGrabbingSunOrMoon = true;
                    targetPosition = mousePosition;
                }

                grabbed.Position = Vector2.Lerp(grabbed.Position, targetPosition, 0.1f);

                if (!Main.mouseLeft || forceRelease)
                {
                    if (!released.ContainsKey(grabbed))
                        released.Add(grabbed, 30);
                    grabbed = null;
                }
            }

            List<CelestialBody> toRemove = new();
            foreach (var kvp in released)
            {
                CelestialBody planet = kvp.Key;
                released[planet]--;
                float releaseProgress = released[planet] / 30f;

                Vector2 directionFromSun = planet.Position - Sun.Position;
                if (released[kvp.Key] <= 0)
                {
                    planet.SetOrbitParent(Sun, directionFromSun.Length(), directionFromSun.ToRotation(), planet.OrbitSpeed);
                    planet.ShouldUpdate = true;
                    toRemove.Add(planet);
                }
                else
                {
                    Vector2 movementTarget = planet.Position + new Vector2(500, 0).RotatedBy((Main.MouseScreen - planet.Position).ToRotation());
                    planet.Position = Vector2.Lerp(planet.Position, movementTarget, 0.01f * releaseProgress);
                }

            }

            foreach (var cb in toRemove)
                released.Remove(cb);
        }
    }
}
