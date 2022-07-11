using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent;
using Macrocosm.Common.Drawing.Stars;
using Macrocosm.Common.Drawing;
using SubworldLibrary;
using Subworlds = Macrocosm.Content.Subworlds.Moon; // ambiguity with this namespace!

namespace Macrocosm.Backgrounds.Moon
{
    public class MoonSky : CustomSky
    {
        public bool Active;
        public float Intensity;

        private static StarsDrawing moonStarsDay = new();
        private static StarsDrawing moonStarsNight = new();

        public static void SpawnStarsOnMoon(bool day)
        {
            if (!SubworldSystem.IsActive<Subworlds.Moon>())
                return;

            if (day)
            {
                // should add some fade-out for these 
                moonStarsDay.Clear(); 
                moonStarsNight.Clear();

                moonStarsDay.SpawnStars(70, 100, 1.6f, 0.05f);
            }
            else
            {
                moonStarsNight.SpawnStars(500, 700, 0.8f, 0.05f);
            }
        }

        public static void DrawMoonNebula(SpriteBatch spriteBatch)
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

            Color nebulaColor;
            float nebulaBrightnessDay = 0.17f;
            float nebulaBrightnessNight = 0.45f;
            float nebulaFade = nebulaBrightnessNight - nebulaBrightnessDay;

            if (Main.dayTime)
            {
                if(Main.time <= 10000)
                {
                    nebulaColor = Color.White * (float)(nebulaBrightnessDay + (Main.time / 10000 * nebulaFade));
                }
                else if(Main.time >= 44000)
                {
                    nebulaColor = Color.White * (float)(nebulaBrightnessDay + (Main.time - 44000) / 10000 * nebulaFade);
                }
                else
                {
                    nebulaColor = Color.White * nebulaBrightnessDay;
                }
            }
            else
            {
                nebulaColor = Color.White * nebulaBrightnessNight;
            }

            nebulaColor.A = 0;

            spriteBatch.Draw(nebula, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), nebulaColor);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SunTexture = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/Sun_0").Value;
            Texture2D SkyTex = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/MoonSky").Value;
            Texture2D earthBody = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/EarthTransparent").Value;
            Texture2D earthAtmo = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Moon/EarthAtmo").Value;

            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * Intensity);

                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)),
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * Intensity));

                DrawMoonNebula(spriteBatch);

                if (moonStarsNight.None && !Main.dayTime) // bugfix bcs we can't check for moon time on subworld OnEnter 
                    SpawnStarsOnMoon(false);

                moonStarsDay.Draw();
                moonStarsNight.Draw();

                //PLEASE DON'T NAME YOUR VARS LIKE "NUM474" EVER AGAIN OR I WILL FCKING RIP YOUR GUTS OUT AND EAT NACHOS FROM YOUR RIBCAGE lol
                float clouldAlphaMult = 1f - Main.cloudAlpha * 1.5f;
                if (clouldAlphaMult < 0f)
                {
                    clouldAlphaMult = 0f;
                }

                int sunAngle = 0;
                int sunTimeX = (int)(Main.time / 54000.0 * (Main.screenWidth + TextureAssets.Sun.Width() * 2)) - TextureAssets.Sun.Width();
                double sunTimeY = Main.time < 27000.0 ? //Gets the Y axis for the angle depending on the time
                        Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0) : //AM
                        Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0); //PM
                float sunScale = 1f;
                float sunRotation = (float)(Main.time / 54000.0) * 2f - 7.3f;
                double bgTop = -Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0;

                if (Main.dayTime)
                {
                    sunAngle = (int)(bgTop + sunTimeY * 250.0 + 180.0);

                    sunScale = (float)(1.2 - sunTimeY * 0.4);

                    Color color6 = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
               
                    Main.spriteBatch.Draw(SunTexture, new Vector2(sunTimeX, sunAngle + Main.sunModY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, SunTexture.Width, SunTexture.Height)),
                        color6, sunRotation, new Vector2(SunTexture.Width / 2, SunTexture.Height / 2), sunScale, SpriteEffects.None, 0f);
                }

                BgCelestialBody.Draw(earthBody, earthAtmo, 0.9f, 0f, -250f, 0.01f, 0.12f);
            }
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