﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent;
using Macrocosm.Common.Drawing.Stars;
using Macrocosm.Common.Drawing;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Backgrounds.Moon
{
    public class MoonSky : CustomSky, ILoadable
    {
        public bool Active;
        public float Intensity;

        private static readonly StarsDrawing moonStarsDay = new();
        private static readonly StarsDrawing moonStarsNight = new();

        const float fadeOutTimeDawn = 7200f; //  4:30 -  6:30: nebula and night stars dim
        const float fadeInTimeDusk = 46800f; // 17:30 - 19:30: nebula and night stars brighten

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            MoonSky moonSky = new MoonSky();
            Filters.Scene["Macrocosm:MoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.High);
            SkyManager.Instance["Macrocosm:MoonSky"] = moonSky;
        }

        public void Unload() { }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D skyTexture = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/MoonSky").Value;
            Texture2D sunTexture = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/Sun_0").Value;
            Texture2D earthBody = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/EarthTransparent").Value;
            Texture2D earthAtmo = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/EarthAtmo").Value;

            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * Intensity);

                spriteBatch.Draw(skyTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * Intensity));

                float nebulaBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.17f, 0.45f); 
                float nightStarBrightness = ComputeBrightness(fadeOutTimeDawn, fadeInTimeDusk, 0.1f, 0.8f);

                DrawMoonNebula(nebulaBrightness);

                moonStarsDay.Draw();
                moonStarsNight.Draw(nightStarBrightness);

                RotatingCelestialBody.Draw(sunTexture, dayTime: true);
                ParallaxingCelestialBody.Draw(earthBody, earthAtmo, 0.9f, 0f, -200f, 0.01f, 0.12f);
            }
        }

        public static void SpawnStarsOnMoon()
        {
            moonStarsDay.SpawnStars(100, 130, 1.4f, 0.05f);
            moonStarsNight.SpawnStars(600, 700, 0.8f, 0.05f);
        }

        public static void ClearStarsOnMoon()
        {
            moonStarsDay.Clear();
            moonStarsNight.Clear();
        }

        private static void DrawMoonNebula(float brightness)
        {
            Texture2D nebula = Main.moonType switch
            {
                1 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaYellow").Value,
                2 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaRinged").Value,
                3 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaMythril").Value,
                4 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaBlue").Value,
                5 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaGreen").Value,
                6 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaPink").Value,
                7 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaOrange").Value,
                8 => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaPurple").Value,
                _ => ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/NebulaNormal").Value,
            };

            Color nebulaColor = Color.White * brightness;
            nebulaColor.A = 0;

            Main.spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
        }

        private static float ComputeBrightness(double fadeOutTimeDawn, double fadeInTimeDusk, float maxBrightnessDay, float maxBrightnessNigt)
        {
            float brightness;

            float fadeFactor = maxBrightnessNigt - maxBrightnessDay;

            if (Main.dayTime)
            {
                if (Main.time <= fadeOutTimeDawn)
                {
                    brightness = (maxBrightnessDay + ((1f - (float)(Main.time / fadeOutTimeDawn)) * fadeFactor));
                }
                else if (Main.time >= fadeInTimeDusk)
                {
                    brightness = (maxBrightnessDay + (float)((Main.time - fadeInTimeDusk) / fadeOutTimeDawn) * fadeFactor);
                }
                else
                {
                    brightness = maxBrightnessDay;
                }
            }
            else
            {
                brightness = maxBrightnessNigt;
            }

            return brightness;
        }

        public override void Update(GameTime gameTime)
        {
            Intensity = Active ? Math.Min(1f, 0.01f + Intensity) : Math.Max(0f, Intensity - 0.01f);
        }

        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
        }


        public override float GetCloudAlpha()
        {
            return 1f - Intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Active = false;
        }

        public override void Reset()
        {
            Active = false;
        }

        public override bool IsActive()
        {
            return Active || Intensity > 0.001f;
        }
    }
}