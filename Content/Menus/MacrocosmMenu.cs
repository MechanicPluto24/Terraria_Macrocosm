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
        private const string path = "Macrocosm/Content/Menus/";
        private const AssetRequestMode immediate = AssetRequestMode.ImmediateLoad;

        private Asset<Texture2D> logo;
        private Asset<Texture2D> logoOld;

        private List<CelestialBody> celestialBodies;
        private List<CelestialBody> planetsWithMoons;
        private List<CelestialBody> interactible;

        private Stars stars;
        private Asset<Texture2D> milkyWay;
        private Asset<Texture2D> nebula;
        private RawTexture milkyWayRaw;
        private RawTexture nebulaRaw;

        private CelestialBody sun;
        private Asset<Texture2D> sunCorona1;
        private Asset<Texture2D> sunCorona2;
        private Asset<Texture2D> sunCorona3;
        private Asset<Texture2D> sunCorona4;
        private Asset<Texture2D> sunCorona5;
        private Asset<Texture2D> flare;
        private Asset<Texture2D> scorch1;
        private Asset<Texture2D> scorch2;
        private Asset<Effect> radialSaturation;

        private CelestialBody vulcan;

        private CelestialBody mercury;

        private CelestialBody venus;

        private CelestialBody earth;
        private CelestialBody luna;

        private CelestialBody mars;
        private CelestialBody phobos;
        private CelestialBody deimos;

        private CelestialBody ceres;
        private Asset<Texture2D> asteroids;
        private List<CelestialBody> asteroidBelt;

        private CelestialBody jupiter;
        private CelestialBody io;
        private CelestialBody europa;

        private CelestialBody saturn;
        private CelestialBody titan;

        private CelestialBody ouranos;
        private CelestialBody miranda;

        private CelestialBody neptune;
        private CelestialBody triton;

        private CelestialBody plutoBarycenter;
        private CelestialBody pluto;
        private CelestialBody charon;

        private CelestialBody eris;

        private Asset<Texture2D> icyAsteroids;
        private List<CelestialBody> kuiperBelt;

        public MacrocosmMenu()
        {
        }

        private CelestialBody grabbed;
        private readonly Dictionary<CelestialBody, int> released = new();
        private readonly List<CelestialBody> destroyed = new();
        private bool drawOldLogo = false;

        public override Asset<Texture2D> Logo => !drawOldLogo ? logo : logoOld;
        public override Asset<Texture2D> SunTexture => Macrocosm.EmptyTex;
        public override Asset<Texture2D> MoonTexture => Macrocosm.EmptyTex;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/IntoTheUnknown");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;
        public override string DisplayName => "Macrocosm";

        public override void OnSelected()
        {
            Setup();
        }

        public override void OnDeselected()
        {
            Clear();
        }

        private void Setup()
        {
            celestialBodies = new();
            planetsWithMoons = new();
            interactible = new();

            logo ??= ModContent.Request<Texture2D>(path + "Logo");
            logoOld ??= ModContent.Request<Texture2D>(path + "Logo_Old");

            milkyWay ??= ModContent.Request<Texture2D>(path + "MilkyWay", immediate);
            nebula ??= ModContent.Request<Texture2D>(path + "Nebula", immediate);

            milkyWayRaw = RawTexture.FromAsset(milkyWay);
            nebulaRaw = RawTexture.FromAsset(nebula);

            stars = new();
            stars.SpawnStars(850, baseScale: 0.8f, randomColor: true);
            stars.SpawnStars(milkyWayRaw, 2000, baseScale: 0.6f);

            sun ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Sun", immediate));
            sun.Center = Utility.ScreenCenter;
            sun.OverrideBodyShader = Sun_GetShader;
            sunCorona1 ??= ModContent.Request<Texture2D>(path + "CelestialBodies/SunCorona1");
            sunCorona2 ??= ModContent.Request<Texture2D>(path + "CelestialBodies/SunCorona2");
            sunCorona3 ??= ModContent.Request<Texture2D>(path + "CelestialBodies/SunCorona3");
            sunCorona4 ??= ModContent.Request<Texture2D>(path + "CelestialBodies/SunCorona4");
            sunCorona5 ??= ModContent.Request<Texture2D>(path + "CelestialBodies/SunCorona5");

            vulcan ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Vulcan", immediate));
            vulcan.SetOrbitParent(sun, 174, Rand(), 0.0022f);

            mercury ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Mercury", immediate));
            mercury.SetOrbitParent(sun, 204, Rand(), 0.0018f);

            venus ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Venus", immediate));
            venus.SetOrbitParent(sun, 238, Rand(), 0.0014f);

            earth ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Earth", immediate));
            earth.SetOrbitParent(sun, 288, Rand(), 0.001f);
            luna ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Luna", immediate));
            luna.SetOrbitParent(earth, new Vector2(36, 10), 0f, Rand(), 0.018f);

            mars ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Mars", immediate));
            mars.SetOrbitParent(sun, 330, Rand(), 0.0008f);
            phobos ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Phobos", immediate));
            phobos.SetOrbitParent(mars, new Vector2(20, 8), 0.2f, Rand(), 0.014f);
            deimos ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Deimos", immediate));
            deimos.SetOrbitParent(mars, new Vector2(24, 10), -0.4f, Rand(), 0.016f);

            ceres = new(ModContent.Request<Texture2D>(path + "CelestialBodies/Ceres", immediate));
            ceres.SetOrbitParent(sun, 362, Rand(), 0.0006f);

            asteroids = ModContent.Request<Texture2D>(path + "CelestialBodies/Asteroids", immediate);
            asteroidBelt = new();
            for (int i = 0; i < 450; i++)
            {
                Rectangle sourceRect = asteroids.Frame(verticalFrames: 8, frameY: Main.rand.Next(8));
                CelestialBody asteroid = new(asteroids, scale: 0.6f, bodySourceRect: sourceRect);
                asteroid.ResetSpritebatch = false;
                asteroid.SetOrbitParent(sun,
                    orbitRadius: 370 + Main.rand.NextFloat(40) * MathF.Sin(Rand()),
                    orbitRotation: Rand(),
                    orbitSpeed: Main.rand.NextFloat(0.00001f, 0.00045f)
                    );
                asteroidBelt.Add(asteroid);
            }

            jupiter ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Jupiter", immediate));
            jupiter.SetOrbitParent(sun, 410, Rand(), 0.0004f);
            io ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Io", immediate));
            io.SetOrbitParent(jupiter, new Vector2(48, 20), 0.2f, Rand(), 0.012f);
            europa ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Europa", immediate));
            europa.SetOrbitParent(jupiter, new Vector2(54, 18), 0.1f, Rand(), 0.01f);

            saturn ??= new(
                ModContent.Request<Texture2D>(path + "CelestialBodies/Saturn", immediate),
                ModContent.Request<Texture2D>(path + "CelestialBodies/SaturnRings", immediate),
                ModContent.Request<Texture2D>(path + "CelestialBodies/SaturnRings", immediate)
            );
            saturn.SetOrbitParent(sun, 514, Rand(), 0.00037f);
            saturn.ConfigureBackRadialShader = Saturn_ConfigureRingsShader;
            saturn.ConfigureFrontRadialShader = Saturn_ConfigureRingsShader;
            saturn.OverrideBackDraw = Saturn_DrawRings_Back;
            saturn.OverrideFrontDraw = Saturn_DrawRings_Front;
            titan ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Titan", immediate));
            titan.SetOrbitParent(saturn, new Vector2(52, 28), 0.8f, Rand(), 0.012f);

            ouranos ??= new(
                ModContent.Request<Texture2D>(path + "CelestialBodies/Ouranos", immediate),
                ModContent.Request<Texture2D>(path + "CelestialBodies/OuranosRings", immediate),
                ModContent.Request<Texture2D>(path + "CelestialBodies/OuranosRings", immediate)
            );

            ouranos.SetOrbitParent(sun, 622, Rand(), 0.0003f);
            ouranos.OverrideBackDraw = Ouranos_DrawRings_Back;
            ouranos.OverrideFrontDraw = Ouranos_DrawRings_Front;
            miranda ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Miranda", immediate));
            miranda.SetOrbitParent(ouranos, new Vector2(42, 18), 0f, Rand(), 0.017f);

            neptune ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Neptune", immediate));
            neptune.SetOrbitParent(sun, 700, Rand(), 0.00027f);
            triton ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Triton", immediate));
            triton.SetOrbitParent(neptune, new Vector2(36, 26), 0.9f, Rand(), 0.018f);

            plutoBarycenter ??= new();
            pluto ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Pluto", immediate));
            plutoBarycenter.AddOrbitChild(pluto, 4, 0f, 0.005f);
            charon ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Charon", immediate));
            plutoBarycenter.AddOrbitChild(charon, 18, 3.14f, 0.008f);
            plutoBarycenter.SetOrbitParent(sun, new Vector2(760, 620), 0.28f, Rand(), 0.00022f);

            eris ??= new(ModContent.Request<Texture2D>(path + "CelestialBodies/Eris", immediate));
            eris.SetOrbitParent(sun, 810, Rand(), 0.00018f);

            icyAsteroids ??= ModContent.Request<Texture2D>(path + "CelestialBodies/IcyAsteroids", immediate);
            kuiperBelt = new();
            for (int i = 0; i < 600; i++)
            {
                Rectangle sourceRect = icyAsteroids.Frame(verticalFrames: 8, frameY: Main.rand.Next(8));
                CelestialBody icyAsteroid = new(icyAsteroids, scale: 0.6f, bodySourceRect: sourceRect);
                icyAsteroid.ResetSpritebatch = false;
                icyAsteroid.SetOrbitParent(sun,
                    orbitRadius: 800 + Main.rand.NextFloat(100) * MathF.Sin(Rand()),
                    orbitRotation: Rand(),
                    orbitSpeed: Main.rand.NextFloat(0.00005f, 0.00015f)
                    );
                kuiperBelt.Add(icyAsteroid);
            }

            celestialBodies =
            [
                vulcan,
                mercury,
                venus,
                earth,
                luna,
                mars,
                phobos,
                deimos,
                ceres,
                jupiter,
                io,
                europa,
                saturn,
                titan,
                ouranos,
                miranda,
                neptune,
                triton,
                pluto,
                charon,
                eris
            ];

            foreach (CelestialBody body in celestialBodies)
            {
                body.ShouldUpdate = true;
                body.Scale = 0.6f;
                body.SetLighting(sun);
                body.ConfigureBodySphericalShader = ConfigureBodyShader;
            }

            planetsWithMoons =
            [
                earth,
                mars,
                jupiter,
                saturn,
                ouranos,
                neptune,
                plutoBarycenter
            ];

            interactible =
            [
                vulcan,
                mercury,
                venus,
                earth,
                mars,
                ceres,
                jupiter,
                saturn,
                ouranos,
                neptune,
                plutoBarycenter,
                eris
            ];
        }

        private static float Rand() => Utility.RandomRotation();

        private void Clear()
        {
            stars.Clear();
            sun.ClearOrbitChildren();
            asteroidBelt.Clear();
            kuiperBelt.Clear();
            released.Clear();
            destroyed.Clear();
        }

        public override void Update(bool isOnTitleScreen)
        {
            drawOldLogo = destroyed.Count > 8;
            Interact();
        }

        private SpriteBatchState state1, state2;
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            if (drawOldLogo)
                logoScale *= 0.45f;
            else
                logoScale *= 0.65f;

            drawColor = Color.White;
            sun.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);
            sun.Scale = 0.85f + 0.01f * Utility.SineWave(500, MathF.PI / 2);

            Rectangle screen = new(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screen, Color.Black);

            state1.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state1);

            stars.DrawAll(spriteBatch);
            spriteBatch.Draw(milkyWay.Value, screen, Color.White.WithOpacity(0.3f));
            spriteBatch.Draw(nebula.Value, screen, Color.White.WithOpacity(0.75f));

            spriteBatch.End();
            spriteBatch.Begin(state1);

            // Draw the asteroids behind everything.
            // They do not reset the spritebatch in the draw logic.
            foreach (CelestialBody asteroid in asteroidBelt)
                asteroid.Draw(spriteBatch);
            foreach (CelestialBody asteroid in kuiperBelt)
                asteroid.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(state1);

            List<CelestialBody> toDraw = planetsWithMoons.Where((planet) => !destroyed.Contains(planet)).ToList();

            // Draw the moons behind the host planet
            foreach (CelestialBody planet in toDraw)
                planet.DrawChildren(spriteBatch, (child) => child.OrbitRotation - MathHelper.Pi >= child.OrbitAngle);

            // Draw the planets behind the Sun
            sun.DrawChildren(spriteBatch, (child) => !(destroyed.Contains(child) || asteroidBelt.Contains(child) || kuiperBelt.Contains(child)));

            // Draw the moons in front of the host planet
            foreach (CelestialBody planet in toDraw)
                planet.DrawChildren(spriteBatch, (child) => child.OrbitRotation - MathHelper.Pi < child.OrbitAngle);

            DrawSunCorona();

            // Draw the Sun and additive effects
            sun.Draw(spriteBatch);
            DrawSunLightEffects(spriteBatch);

            return true;
        }

        private float SolarFlareProgress => Utility.PositiveSineWave(450, MathF.PI / 2);

        private void DrawSunCorona()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            state2.SaveState(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(BlendState.AlphaBlend, state2);

            spriteBatch.Draw(sunCorona1.Value, sun.Center, null, (new Color(127, 127, 127, 127) * (0.4f + 0.8f * Utility.PositiveSineWave(800, 0f))), 0, sunCorona1.Size() / 2, 0.85f + 0.04f * Utility.SineWave(800, 0f), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona2.Value, sun.Center, null, (new Color(127, 127, 127, 127) * (0.6f + 0.4f * Utility.PositiveSineWave(600, MathF.PI / 8))), 0, sunCorona1.Size() / 2, 0.85f + 0.03f * Utility.SineWave(600, MathF.PI / 8), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona3.Value, sun.Center, null, (new Color(127, 127, 127, 127) * (0.8f + 0.2f * Utility.PositiveSineWave(500, MathF.PI / 4))), 0, sunCorona1.Size() / 2, 0.85f + 0.03f * Utility.SineWave(500, MathF.PI / 3), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona4.Value, sun.Center, null, (new Color(127, 127, 127, 127) * (0.7f + 0.3f * Utility.PositiveSineWave(500, MathF.PI / 2))), 0, sunCorona1.Size() / 2, 0.85f + 0.02f * Utility.SineWave(500, MathF.PI / 2), SpriteEffects.None, 0f);
            spriteBatch.Draw(sunCorona5.Value, sun.Center, null, (new Color(127, 127, 127, 127) * (0.6f + 0.4f * Utility.PositiveSineWave(300, MathF.PI / 2))), 0, sunCorona1.Size() / 2, 0.85f * 0.95f + 0.02f * Utility.SineWave(300, MathF.PI / 2), SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state2);
        }

        private void DrawSunLightEffects(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state1);
            flare ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "HighRes/Flare3");
            scorch1 ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "HighRes/Scorch1");
            scorch2 ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "HighRes/Scorch2");
            spriteBatch.Draw(flare.Value, sun.Center, null, new Color(255, 96, 4) * (0.9f + 0.1f * SolarFlareProgress), 0f, flare.Size() / 2f, 2.2f + 0.2f * SolarFlareProgress, SpriteEffects.None, 0);
            spriteBatch.Draw(scorch1.Value, sun.Center, null, new Color(255, 193, 0) * (0.25f + 0.01f * SolarFlareProgress), MathHelper.TwoPi * Utility.PositiveTriangleWave(15000), scorch1.Size() / 2f, 2.5f + 0.01f * SolarFlareProgress, SpriteEffects.None, 0);
            spriteBatch.Draw(scorch2.Value, sun.Center, null, new Color(255, 193, 0) * (0.2f + 0.01f * SolarFlareProgress), MathHelper.TwoPi * -Utility.PositiveTriangleWave(15000), scorch2.Size() / 2f, 2.5f + 0.01f * SolarFlareProgress, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(state1);
        }

        private void ConfigureBodyShader(CelestialBody celestialBody, CelestialBody lightSource, out Vector3 lightPosition, out float radius, out int pixelSize)
        {
            float rotationProgress = Math.Abs(MathHelper.WrapAngle(celestialBody.OrbitRotation + MathHelper.PiOver2)) / MathHelper.Pi;
            float depth = MathHelper.Lerp(500, -50, Utility.CubicEaseInOut(rotationProgress));
            lightPosition = new Vector3(lightSource.Center, depth);
            radius = 0.01f;
            pixelSize = 1;
        }

        private void ConfigureBodyShader(CelestialBody celestialBody, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
        {
            float rotationProgress = Math.Abs(MathF.Cos((rotation + MathHelper.Pi)));
            intensity = 1f;
            float offsetRadius = MathHelper.Lerp(0.5f, 0.3f, Utility.QuadraticEaseInOut(rotationProgress));
            offset = Utility.PolarVector(offsetRadius, rotation);
            shadeResolution *= 2;
            radius = MathHelper.Lerp(1.8f, 1.3f, Math.Abs(rotationProgress)) - 0.15f * SolarFlareProgress;
        }

        private Effect Sun_GetShader()
        {
            radialSaturation ??= ModContent.Request<Effect>(Macrocosm.ShadersPath + "RadialSaturation", immediate);
            Effect effect = radialSaturation.Value;
            effect.Parameters["uCenter"].SetValue(Vector2.One * 0.5f);
            effect.Parameters["uRadius"].SetValue(0.2f + 0.1f * SolarFlareProgress);
            effect.Parameters["uIntensity"].SetValue(0.2f + 0.1f * SolarFlareProgress);
            return effect;
        }

        private void Saturn_DrawRings_Back(CelestialBody saturn, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, Saturn_GetMatrix(saturn, state.Matrix));

            Rectangle sourceRect = new(0, 0, rings.Width(), rings.Height() / 2);
            Vector2 position = saturn.Center;
            spriteBatch.Draw(rings.Value, position, sourceRect, saturn.Color, 0f, new Vector2(sourceRect.Width / 2, sourceRect.Height), saturn.Scale * 0.96f, default, 0f);

            spriteBatch.End();
        }

        private void Saturn_DrawRings_Front(CelestialBody saturn, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, Saturn_GetMatrix(saturn, state.Matrix));

            Rectangle sourceRect = new(0, rings.Height() / 2, rings.Width(), rings.Height() / 2);
            Vector2 position = saturn.Center + new Vector2(0, 0);
            spriteBatch.Draw(rings.Value, position, sourceRect, saturn.Color, 0f, new Vector2(rings.Height() / 2f, 0), saturn.Scale * 0.96f, default, 0f);

            spriteBatch.End();
        }

        private static Matrix Saturn_GetMatrix(CelestialBody saturn, Matrix uiScaleMatrix)
        {
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-saturn.Center.X, -saturn.Center.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationX(MathHelper.Pi * 0.29f) *                    // Apply X skew 
                Matrix.CreateRotationY(MathHelper.Pi * 0.3f) *                     // Apply Y skew
                Matrix.CreateTranslation(saturn.Center.X, saturn.Center.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(uiScaleMatrix.M11, uiScaleMatrix.M22, 0f);      // Apply UI scale
            return transformationMatrix;
        }

        private void Saturn_ConfigureRingsShader(CelestialBody saturn, float rotation, out float intensity, out Vector2 offset, out float radius, ref Vector2 shadeResolution)
        {
            intensity = -1f;
            float rotationProgress = Math.Abs(MathF.Cos((rotation + MathHelper.Pi)));
            float offsetRadius = MathHelper.Lerp(-0.45f, -0.4f, Utility.QuadraticEaseInOut(rotationProgress));
            offset = Utility.PolarVector(offsetRadius, rotation + MathHelper.Pi / 4);
            radius = MathHelper.Lerp(0.6f, 0.6f, Utility.QuadraticEaseInOut(rotationProgress));
        }

        private void Ouranos_DrawRings_Back(CelestialBody ouranos, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, Ouranos_GetMatrix(ouranos, state.Matrix));

            Rectangle sourceRect = new(0, 0, rings.Width(), rings.Height() / 2);
            Vector2 position = ouranos.Center;
            spriteBatch.Draw(rings.Value, position, sourceRect, ouranos.Color, MathHelper.PiOver2, new Vector2(sourceRect.Width / 2, sourceRect.Height), ouranos.Scale * 1f, default, 0f);

            spriteBatch.End();
        }

        private void Ouranos_DrawRings_Front(CelestialBody ouranos, SpriteBatch spriteBatch, SpriteBatchState state, Asset<Texture2D> rings, Effect shader)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, shader, Ouranos_GetMatrix(ouranos, state.Matrix));

            Rectangle sourceRect = new(0, rings.Height() / 2, rings.Width(), rings.Height() / 2);
            Vector2 position = ouranos.Center + new Vector2(0, 0);
            spriteBatch.Draw(rings.Value, position, sourceRect, ouranos.Color, MathHelper.PiOver2, new Vector2(rings.Height() / 2f, 0), ouranos.Scale * 1f, default, 0f);

            spriteBatch.End();
        }

        private static Matrix Ouranos_GetMatrix(CelestialBody ouranos, Matrix uiScaleMatrix)
        {
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-ouranos.Center.X, -ouranos.Center.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationX(MathHelper.Pi * 0.2f) *                           // Apply X skew 
                Matrix.CreateRotationY(MathHelper.Pi * 0.4f) *                           // Apply Y skew
                Matrix.CreateRotationZ(MathHelper.Pi * 0.3f) *                           // Apply Z skew
                Matrix.CreateTranslation(ouranos.Center.X, ouranos.Center.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(uiScaleMatrix.M11, uiScaleMatrix.M22, 0f);            // Apply UI scale
            return transformationMatrix;
        }
        private void Interact()
        {
            Main.alreadyGrabbingSunOrMoon = false;

            if (grabbed is null && released.Count == 0)
            {
                foreach (var celestialBody in interactible.Where((planet) => !destroyed.Contains(planet)))
                {
                    if (celestialBody.Hitbox.Contains(Main.mouseX, Main.mouseY) && Main.mouseLeft)
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
                bool forceRelease = false;

                Main.alreadyGrabbingSunOrMoon = true;
                targetPosition = mousePosition;
                grabbed.Center = Vector2.Lerp(grabbed.Center, targetPosition, 0.1f);
                grabbed.OrbitRotation = (grabbed.Center - sun.Center).ToRotation();

                float minDistance = 120f - (grabbed.Width * grabbed.Scale);
                if (Vector2.DistanceSquared(mousePosition, sun.Center) < minDistance * minDistance)
                {
                    destroyed.Add(grabbed);
                    grabbed = null;
                }

                if (!Main.mouseLeft || forceRelease)
                {
                    released.TryAdd(grabbed, 1);
                    grabbed = null;
                }
            }

            List<CelestialBody> toRemove = new();
            foreach (var kvp in released)
            {
                CelestialBody planet = kvp.Key;
                released[planet]--;
                float releaseProgress = released[planet] / 30f;

                Vector2 directionFromSun = planet.Center - sun.Center;
                if (released[kvp.Key] <= 0)
                {
                    planet.SetOrbitParent(sun, directionFromSun.Length(), directionFromSun.ToRotation(), planet.OrbitSpeed);
                    planet.ShouldUpdate = true;
                    toRemove.Add(planet);
                }
                else
                {
                    Vector2 movementTarget = planet.Center + new Vector2(500, 0).RotatedBy(planet.OrbitRotation + MathHelper.PiOver2);
                    planet.Center = Vector2.Lerp(planet.Center, movementTarget, 0.01f * releaseProgress);
                }
            }

            foreach (var cb in toRemove)
                released.Remove(cb);
        }
    }
}
