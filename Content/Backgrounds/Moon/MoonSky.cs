using Macrocosm.Common.Drawing.Sky;
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

namespace Macrocosm.Content.Backgrounds.Moon
{
    public class MoonSky : CustomSky, ILoadable
    {
        public bool Active;
        public float Intensity;

        private Stars starsDay;
        private Stars starsNight;

        private CelestialBody earth;
        private CelestialBody sun;

        private Texture2D skyTexture;

        private Texture2D sunTexture;

        private Texture2D earthBody;
        private Texture2D earthBodyDrunk;
        private Texture2D earthBodyFlat;

        private Texture2D earthAtmo;
        private Texture2D earthBodyShadow;
        private Texture2D earthAtmoShadow;

        const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
        const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

        const string AssetPath = "Macrocosm/Content/Backgrounds/Moon/";

        public MoonSky()
        {
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
            skyTexture = ModContent.Request<Texture2D>(AssetPath + "MoonSky", mode).Value;

            sunTexture = ModContent.Request<Texture2D>(AssetPath + "Sun", mode).Value;
            earthBody = ModContent.Request<Texture2D>(AssetPath + "Earth", mode).Value;
            earthBodyDrunk = ModContent.Request<Texture2D>(AssetPath + "EarthDrunk", mode).Value;
            earthBodyFlat = ModContent.Request<Texture2D>(AssetPath + "EarthFlat", mode).Value;

            earthAtmo = ModContent.Request<Texture2D>(AssetPath + "EarthAtmo", mode).Value;
            earthBodyShadow = ModContent.Request<Texture2D>(AssetPath + "EarthShadowMask", mode).Value;
            earthAtmoShadow = ModContent.Request<Texture2D>(AssetPath + "EarthAtmoShadowMask", mode).Value;

            starsDay = new();
            starsNight = new();

            sun = new CelestialBody(sunTexture);
            earth = new CelestialBody(earthBody, earthAtmo, 0.9f);

            sun.SetupSkyRotation(SkyRotationMode.Day);

            earth.SetParallax(0.01f, 0.12f, new Vector2(0f, -200f));

            earth.SetLightSource(sun);
            earth.ConfigureShader = (float rotation, out float intensity, out Vector2 offset) =>
            {
                Vector2 screenSize = Main.ScreenSize.ToVector2();
                float distance = Vector2.Distance(earth.Position / screenSize, earth.LightSource.Position / screenSize);
                float offsetRadius = MathHelper.Lerp(0.12f, 0.56f, 1 - distance);

                if (!Main.dayTime)
                {
                    offsetRadius = MathHelper.Lerp(0.56f, 0.01f, 1 - distance);
                    rotation += MathHelper.Pi;
                }

                offset = Utility.PolarVector(offsetRadius, rotation) * 0.65f;

                intensity = 0.96f;
            };
        }

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            SkyManager.Instance["Macrocosm:MoonSky"] = new MoonSky();
        }

        public void Unload() { }

        public override void Activate(Vector2 position, params object[] args)
        {
            SkyManager.Instance["Macrocosm:MoonSky"] = new MoonSky();

            starsDay.SpawnStars(100, 130, baseScale: 1.4f, twinkleFactor: 0.05f);
            starsNight.SpawnStars(600, 700, baseScale: 0.8f, twinkleFactor: 0.05f);

            MacrocosmStar mars = starsDay.RandStar(); // :) 
            mars.OverrideColor(new Color(224, 137, 8, 220));
            mars.Scale *= 1.4f;

            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            starsDay.Clear();
            starsNight.Clear();
            Active = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.IsActive<Subworlds.Moon>() && maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Main.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Draw(skyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * Intensity);

                float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.17f, 0.45f);
                float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.1f, 0.8f);

                DrawMoonNebula(nebulaBrightness);

                starsDay.Draw(spriteBatch);
                starsNight.Draw(spriteBatch, nightStarBrightness);

                sun.Draw(spriteBatch);
                earth.Draw(spriteBatch);
            }
        }

        private static void DrawMoonNebula(float brightness)
        {
            Texture2D nebula = Main.moonType switch
            {
                1 => ModContent.Request<Texture2D>(AssetPath + "NebulaYellow").Value,
                2 => ModContent.Request<Texture2D>(AssetPath + "NebulaRinged").Value,
                3 => ModContent.Request<Texture2D>(AssetPath + "NebulaMythril").Value,
                4 => ModContent.Request<Texture2D>(AssetPath + "NebulaBlue").Value,
                5 => ModContent.Request<Texture2D>(AssetPath + "NebulaGreen").Value,
                6 => ModContent.Request<Texture2D>(AssetPath + "NebulaPink").Value,
                7 => ModContent.Request<Texture2D>(AssetPath + "NebulaOrange").Value,
                8 => ModContent.Request<Texture2D>(AssetPath + "NebulaPurple").Value,
                _ => ModContent.Request<Texture2D>(AssetPath + "NebulaNormal").Value,
            };

            Color nebulaColor = (Color.White * brightness).WithOpacity(0f);

            Main.spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
        }

        public override void Update(GameTime gameTime)
        {
            Intensity = Active ? Math.Min(1f, 0.01f + Intensity) : Math.Max(0f, Intensity - 0.01f);
            SetEarthTextures();
        }

        private void SetEarthTextures()
        {
            if (Utility.IsAprilFools())
            {
                earth.SetLightSource(null);
                earth.SetTextures(earthBodyFlat);
            }
            else
            {
                earth.SetLightSource(sun);

                if (Main.drunkWorld)
                    earth.SetTextures(earthBodyDrunk, earthAtmo, earthBodyShadow, earthAtmoShadow);
                else
                    earth.SetTextures(earthBody, earthAtmo, earthBodyShadow, earthAtmoShadow);
            }
        }

        private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
        {
            float brightness;

            float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

            if (Main.dayTime)
            {
                if (Main.time <= fadeOutTimeDawn)
                    brightness = (maxBrightnessDay + ((1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor));
                else if (Main.time >= fadeInTimeDusk)
                    brightness = (maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor);
                else
                    brightness = maxBrightnessDay;
            }
            else
            {
                brightness = maxBrightnessNigt;
            }

            return brightness;
        }


        public override Color OnTileColor(Color inColor)
        {
            Color color = inColor.ToGrayscale();
            return Color.Lerp(color, Color.Black, 0.2f + Intensity * 0.1f);
        }

        public override float GetCloudAlpha() => 0f;

        public override void Reset()
        {
            starsDay.Clear();
            starsNight.Clear();
            Active = false;
        }

        public override bool IsActive()
        {
            return Active || Intensity > 0.001f;
        }
    }
}