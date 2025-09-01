using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Content.Skies.Moon
{
        // Only load visuals on the client.
    [Autoload(Side = ModSide.Client)]
    public class MoonSky : CustomSky, ILoadable
    {
        private bool active;
        private float intensity;

        private readonly Stars starsDay;
        private readonly Stars starsNight;

        private CelestialBody earth;
        private readonly CelestialBodySprite sun;

        private readonly Asset<Texture2D> skyTexture;

        private readonly Asset<Texture2D> sunTexture;

        private readonly Asset<Texture2D> earthBody;
        private readonly Asset<Texture2D> earthBodyDrunk;
        private readonly Asset<Texture2D> earthBodyFlat;
        private readonly Asset<Texture2D> earthAtmo;

        private readonly Asset<Texture2D> earthMercator;
        private readonly Asset<Texture2D> earthMercatorOverlay;

        private readonly Asset<Texture2D>[] nebulaTextures = new Asset<Texture2D>[Main.maxMoons];
        private readonly RawTexture[] nebulaRawTextures = new RawTexture[Main.maxMoons];

        private const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
        private const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

        private bool shouldRefreshNebulaStars = true;
        private int lastMoonType = 0;

        private const string Path = "Macrocosm/Content/Skies/Moon/";

        public MoonSky()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            skyTexture = ModContent.Request<Texture2D>(Path + "MoonSky", mode);

            sunTexture = ModContent.Request<Texture2D>(Path + "Sun", mode);
            earthBody = ModContent.Request<Texture2D>(Path + "Earth", mode);
            earthBodyDrunk = ModContent.Request<Texture2D>(Path + "EarthDrunk", mode);
            earthBodyFlat = ModContent.Request<Texture2D>(Path + "EarthFlat", mode);
            earthAtmo = ModContent.Request<Texture2D>(Path + "EarthAtmo", mode);

            earthMercator = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_Mercator");
            earthMercatorOverlay = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/3D/Earth_MercatorClouds");

            starsDay = new();
            starsNight = new();

            sun = new(sunTexture);

            SetupEarth(this, EventArgs.Empty);

                // Allow the type of earth to be changed without requiring a reload.
            ClientConfig.Instance.OnConfigChanged += SetupEarth;

            sun.SetupSkyRotation(CelestialBody.SkyRotationMode.Day);

            string[] nebulaNames =
            [
                "NebulaNormal",
                "NebulaYellow",
                "NebulaRinged",
                "NebulaMythril",
                "NebulaBlue",
                "NebulaGreen",
                "NebulaPink",
                "NebulaOrange",
                "NebulaPurple"
            ];

            for (int i = 0; i < Main.maxMoons; i++)
                nebulaTextures[i] = ModContent.Request<Texture2D>($"{Path}{nebulaNames[i]}", mode);

            for (int i = 0; i < Main.maxMoons; i++)
                nebulaRawTextures[i] = RawTexture.FromAsset(nebulaTextures[i]);
        }

        private void SetupEarth(object sender, EventArgs e)
        {
            if (ClientConfig.Instance.Use3DCelestialBodies)
            {
                earth = new CelestialBodySphere(earthMercator, new(190), projectionOverlayTexture: earthMercatorOverlay)
                    { ConfigureSphereShader = ConfigureEarthSphereShader };
            }
            else
                earth = new CelestialBodySprite(earthBody, earthAtmo, scale: 0.9f);

            earth.SetParallax(0.01f, 0.12f, new Vector2(0f, -200f));

            earth.SetLighting(sun);
        }

        public void Load(Mod mod) =>
            SkyManager.Instance["Macrocosm:MoonSky"] = new MoonSky();

        public void Unload() { }

        public override void Reset()
        {
        }

        public override bool IsActive() => active;

        public override void Activate(Vector2 position, params object[] args)
        {
            intensity = 0.002f;
            active = true;
        }

        public override void Deactivate(params object[] args)
        {
            intensity = 0f;
            starsDay.Clear();
            starsNight.Clear();
            active = false;
            shouldRefreshNebulaStars = true;
        }

        public override float GetCloudAlpha() => 0f;

        public Color GetDemonSunDayColour()
        {
            Color darkColor = new Color(35, 35, 35);
            Color earthshineBlue = Color.Lerp(new Color(39, 87, 155), darkColor, 0.6f);
            if (Main.time < MacrocosmSubworld.GetDayLength() * 0.1)
                return Color.Lerp(Color.Lerp(darkColor, Color.White, (float)(Main.time / (MacrocosmSubworld.GetDayLength() * 0.1))), new Color(150, 80, 80), Subworlds.Moon.Instance.DemonSunVisualIntensity);
            else if (Main.time > MacrocosmSubworld.GetDayLength() * 0.9)
                return Color.Lerp(Color.Lerp(darkColor, Color.White, (float)((MacrocosmSubworld.GetDayLength() - Main.time) / (MacrocosmSubworld.GetDayLength() - MacrocosmSubworld.GetDayLength() * 0.9))), new Color(150, 80, 80), Subworlds.Moon.Instance.DemonSunVisualIntensity);
            else
                return Color.Lerp(Color.White, new Color(150, 80, 80), Subworlds.Moon.Instance.DemonSunVisualIntensity);
        }

        public Color GetDemonSunNightColour()
        {
            Color darkColor = new Color(35, 35, 35);
            Color earthshineBlue = Color.Lerp(new Color(39, 87, 155), darkColor, 0.6f);
            if (Main.time < MacrocosmSubworld.GetNightLength() * 0.2)
                return Color.Lerp(Color.Lerp(darkColor, earthshineBlue, (float)(Main.time / (MacrocosmSubworld.GetNightLength() * 0.2))), new Color(40, 12, 12), Subworlds.Moon.Instance.DemonSunVisualIntensity);
            else if (Main.time > MacrocosmSubworld.GetNightLength() * 0.8)
                return Color.Lerp(Color.Lerp(darkColor, earthshineBlue, (float)((MacrocosmSubworld.GetNightLength() - Main.time) / (MacrocosmSubworld.GetNightLength() - MacrocosmSubworld.GetNightLength() * 0.8))), new Color(40, 12, 12), Subworlds.Moon.Instance.DemonSunVisualIntensity);
            else
                return Color.Lerp(earthshineBlue, new Color(40, 12, 12), Subworlds.Moon.Instance.DemonSunVisualIntensity);
        }


        public override Color OnTileColor(Color inColor)
        {
            Color darkColor = new Color(35, 35, 35);
            Color earthshineBlue = Color.Lerp(new Color(39, 87, 155), darkColor, 0.6f);

            if (Main.dayTime)
            {
                if (WorldData.DemonSun)
                    return GetDemonSunDayColour();

                if (Main.time < MacrocosmSubworld.GetDayLength() * 0.1)
                    return Color.Lerp(darkColor, Color.White, (float)(Main.time / (MacrocosmSubworld.GetDayLength() * 0.1)));
                else if (Main.time > MacrocosmSubworld.GetDayLength() * 0.9)
                    return Color.Lerp(darkColor, Color.White, (float)((MacrocosmSubworld.GetDayLength() - Main.time) / (MacrocosmSubworld.GetDayLength() - MacrocosmSubworld.GetDayLength() * 0.9)));
                else
                    return Color.White;

            }
            else
            {
                if (WorldData.DemonSun)
                    return GetDemonSunNightColour();

                if (Main.time < MacrocosmSubworld.GetNightLength() * 0.2)
                    return Color.Lerp(darkColor, earthshineBlue, (float)(Main.time / (MacrocosmSubworld.GetNightLength() * 0.2)));
                else if (Main.time > MacrocosmSubworld.GetNightLength() * 0.8)
                    return Color.Lerp(darkColor, earthshineBlue, (float)((MacrocosmSubworld.GetNightLength() - Main.time) / (MacrocosmSubworld.GetNightLength() - MacrocosmSubworld.GetNightLength() * 0.8)));
                else
                    return earthshineBlue;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.IsActive<Subworlds.Moon>() && maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Main.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Draw(skyTexture.Value, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * intensity);

                float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.17f, 0.45f);
                float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0f, 0.8f);

                DrawMoonNebula(nebulaBrightness);

                UpdateNebulaStars();

                starsDay.DrawAll(spriteBatch);
                starsNight.DrawAll(spriteBatch, nightStarBrightness);

                sun.Color = new Color((int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity)), (int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity)), (int)(255 * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity))) * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity);

                if (WorldData.DemonSun && Main.dayTime)
                    DrawDemonSunEffects(spriteBatch, sun);

                sun.Draw(spriteBatch);

                //if (WorldFlags.DemonSun&&Main.dayTime)
                //    DrawDemonSunFrontEffects(spriteBatch, sun);

                earth.Draw(spriteBatch);

                /*
                rift ??= new Rift(
                        Main.graphics.GraphicsDevice,  
                        new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), 
                        800f, 
                        400f,  
                        Color.Purple,  
                        Color.Red,  
                        128
                    );

                state1.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Matrix.Identity * Matrix.CreateScale(Main.BackgroundViewMatrix.Zoom.X));
                rift.Draw(Main.Assets.Request<Texture2D>("Images/Misc/noise").Value, state1.Matrix);
                spriteBatch.End();
                spriteBatch.Begin(state1);
                */
            }
        }

        private void UpdateNebulaStars()
        {
            if (lastMoonType != Main.moonType)
                shouldRefreshNebulaStars = true;

            lastMoonType = Main.moonType;

            if (shouldRefreshNebulaStars)
            {
                starsDay.Clear();
                starsNight.Clear();

                starsDay.SpawnStars(120, baseScale: 1.4f, twinkleFactor: 0.05f);
                starsNight.SpawnStars(650, randomColor: true, baseScale: 0.8f, twinkleFactor: 0.05f);

                MacrocosmStar mars = starsDay.RandStar(); // :) 
                mars.Color = new Color(224, 137, 8);

                starsDay?.SpawnStars(colorMap: nebulaRawTextures[Main.moonType], 800, null, baseScale: 1.1f, twinkleFactor: 0.05f);
                starsNight?.SpawnStars(colorMap: nebulaRawTextures[Main.moonType], 6000, null, baseScale: 0.6f, twinkleFactor: 0.05f);

                shouldRefreshNebulaStars = false;
            }
        }

        private void DrawDemonSunEffects(SpriteBatch spriteBatch, CelestialBody Sun)
        {
            float intensity = Subworlds.Moon.Instance.DemonSunVisualIntensity;
            var flare = ModContent.Request<Texture2D>(Macrocosm.FancyHighResTexturesPath + "Flare3").Value;
            var scorch1 = ModContent.Request<Texture2D>(Macrocosm.FancyHighResTexturesPath + "Scorch1").Value;
            var scorch2 = ModContent.Request<Texture2D>(Macrocosm.FancyHighResTexturesPath + "Scorch2").Value;
            var sun = ModContent.Request<Texture2D>(Path + "DemonSunBack").Value;
            var beam = ModContent.Request<Texture2D>(Macrocosm.FancyHighResTexturesPath + "Scratch2").Value;
            float pulse = Utility.PositiveSineWave(450, MathF.PI / 2);
            spriteBatch.Draw(sun, Sun.Center, null, new Color(255, 255, 255, 0) * intensity, MathHelper.TwoPi * -Utility.PositiveTriangleWave(15000), sun.Size() / 2f, (1f) * intensity, SpriteEffects.None, 0);

            spriteBatch.Draw(scorch1, Sun.Center, null, new Color(255, (int)(193 * (1f - intensity)), 0, 0) * (0.4f + 0.01f * pulse) * intensity, MathHelper.TwoPi * Utility.PositiveTriangleWave(15000), scorch1.Size() / 2f, (1.1f + 0.01f * pulse) * intensity, SpriteEffects.None, 0);
            spriteBatch.Draw(scorch2, Sun.Center, null, new Color(255, (int)(193 * (1f - intensity)), 0, 0) * (0.35f + 0.01f * pulse) * intensity, MathHelper.TwoPi * -Utility.PositiveTriangleWave(15000), scorch2.Size() / 2f, (1.1f + 0.01f * pulse) * intensity, SpriteEffects.None, 0);

            spriteBatch.Draw(flare, Sun.Center, null, new Color(255, (int)(193 * (1f - intensity)), 0, 0) * intensity * 0.5f, 0f, flare.Size() / 2f, (2.5f) * intensity, SpriteEffects.None, 0);
            spriteBatch.Draw(beam, Sun.Center, null, new Color(255, 255, 255, 0) * intensity, MathHelper.Pi / 4, beam.Size() / 2f, 0.8f * intensity, SpriteEffects.None, 0);
        }

        private void DrawDemonSunFrontEffects(SpriteBatch spriteBatch, CelestialBody Sun)
        {
            float intensity = Subworlds.Moon.Instance.DemonSunVisualIntensity;
            var circle = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "LowRes/Circle7").Value;
            var portal = ModContent.Request<Texture2D>(Path + "DemonSunFront").Value;
            spriteBatch.Draw(portal, Sun.Center, null, new Color(255, 255, 255) * intensity, 0f, portal.Size() / 2f, 1f * intensity, SpriteEffects.None, 0);
        }

        private void DrawMoonNebula(float brightness)
        {
            Texture2D nebula = nebulaTextures[Main.moonType].Value;
            Color nebulaColor = (Color.White * brightness).WithAlpha(0);

            float bgTopY = (float)(-(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 50.0);
            Main.spriteBatch.Draw(nebula, new System.Drawing.RectangleF(0, bgTopY, Main.screenWidth, Main.screenHeight), nebulaColor);
        }

        public override void Update(GameTime gameTime)
        {
            if (!SubworldSystem.IsActive<Subworlds.Moon>())
                active = false;

            sun.Color = new Color(255, 255, 255) * (1f - Subworlds.Moon.Instance.DemonSunVisualIntensity);

            earth.Color = new Color(255, (int)(255 * (1f - (Subworlds.Moon.Instance.DemonSunVisualIntensity * 0.6f))), (int)(255 * (1f - (Subworlds.Moon.Instance.DemonSunVisualIntensity * 0.6f))));
            intensity = active ? Math.Min(1f, intensity + 0.01f) : Math.Max(0f, intensity - 0.01f);
            UpdateTextures();

            float bgTopY = (float)(-(Main.screenPosition.Y - Main.screenHeight / 2) / (Main.worldSurface * 16.0 - 600.0) * 50.0);
            starsDay.CommonOffset = new Vector2(0, bgTopY);
            starsNight.CommonOffset = new Vector2(0, bgTopY);
        }

        private void UpdateTextures()
        {
            if (Utility.IsAprilFools)
            {
                earth.SetLighting(null);
                //earth.SetTextures(earthBodyFlat);
            }
            else
            {
                earth.SetLighting(sun);

                //if (Main.drunkWorld)
                    //earth.SetTextures(earthBodyDrunk, earthAtmo);
                //else
                    //earth.SetTextures(earthBody, earthAtmo);
            }

            if (Main.LocalPlayer.head == 12)
                sun.SetTextures(TextureAssets.Sun2);
            else
                sun.SetTextures(sunTexture);
        }

        private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
        {
            float brightness;

            float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

            if (Main.dayTime)
            {
                if (Main.time <= fadeOutTimeDawn)
                    brightness = maxBrightnessDay + (1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor;
                else if (Main.time >= fadeInTimeDusk)
                    brightness = maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor;
                else
                    brightness = maxBrightnessDay;
            }
            else
            {
                brightness = maxBrightnessNigt;
            }

            return brightness;
        }

        private void ConfigureEarthSphereShader(
            CelestialBodySphere celestialBody,
            CelestialBody lightSource,
            out Vector3 lightPosition,
            out Vector4 lightColor,
            out Vector4 shadowColor,
            out Vector4 lightAtmosphereColor,
            out Vector4 shadowAtmosphereColor,
            out Matrix bodyRotation,
            out float radius)
        {
            float distanceFactor;
            float depth;
            if (Main.dayTime)
            {
                distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
                depth = MathHelper.Lerp(-60, 400, Utility.QuadraticEaseIn(distanceFactor));
                lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
            }
            else
            {
                distanceFactor = MathHelper.Clamp(Vector2.Distance(celestialBody.Center, lightSource.Center) / Math.Max(celestialBody.Width * 2, celestialBody.Height * 2), 0, 1);
                depth = MathHelper.Lerp(400, 5000, 1f - Utility.QuadraticEaseOut(distanceFactor));
                lightPosition = new Vector3(Utility.ClampOutsideCircle(lightSource.Center, celestialBody.Center, earth.Width / 2 * 2), depth);
            }

                // These are test/dummy values feel free to mess with these all you like.
            lightColor = Vector4.One;
            shadowColor = Color.Black.ToVector4();

            bodyRotation =
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateRotationY(MathHelper.Pi) *
                Matrix.CreateRotationZ(Main.GlobalTimeWrappedHourly * .2f);

            lightAtmosphereColor = Color.Blue.ToVector4();
            shadowAtmosphereColor = Color.DarkRed.ToVector4() * .6f;

                // Value from 0-1 representing the ratio between the atmosphere and actual body.
            radius = .9f;
        }
    }
}