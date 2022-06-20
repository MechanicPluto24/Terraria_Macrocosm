using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Utilities;
using Macrocosm.Content;
using Terraria.GameContent;

namespace Macrocosm.Backgrounds {
    public class MoonSky : CustomSky {
        public bool Active;
        public float Intensity;

        private readonly UnifiedRandom _random = new UnifiedRandom();

        public override void Update(GameTime gameTime) {
            Intensity = Active ? Math.Min(1f, 0.01f + Intensity) : Math.Max(0f, Intensity - 0.01f);
        }

        public override Color OnTileColor(Color inColor) {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth) {
            Texture2D SunTexture = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/Sun_0").Value;
            Texture2D SkyTex = ModContent.Request<Texture2D>("Macrocosm/Backgrounds/MoonSky").Value;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f) {
                spriteBatch.Draw((Texture2D)TextureAssets.BlackTile, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * Intensity);
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), 
                    Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * Intensity));
                //PLEASE DON'T NAME YOUR VARS LIKE "NUM474" EVER AGAIN OR I WILL FCKING RIP YOUR GUTS OUT AND EAT NACHOS FROM YOUR RIBCAGE lol
                float clouldAlphaMult = 1f - Main.cloudAlpha * 1.5f;
                if (clouldAlphaMult < 0f) {
                    clouldAlphaMult = 0f;
                }
                int sunAngle = 0;
                int sunTimeX = (int)(Main.time / 54000.0 * (Main.screenWidth + TextureAssets.Sun.Width() * 2)) - TextureAssets.Sun.Width();
                double sunTimeY = Main.time < 27000.0 ? //Gets the Y axis for the angle depending on the time
                        Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0) : //AM
                        Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0); //PM
                float sunScale = 1f;
                float sunRotation = (float)(Main.time / 54000.0) * 2f - 7.3f;
                double bgTop = (-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0;
                if (Main.dayTime) {
                    sunAngle = (int)(bgTop + sunTimeY * 250.0 + 180.0);

                    sunScale = (float)(1.2 - sunTimeY * 0.4);
                }
                Color color6 = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
                Main.spriteBatch.Draw(SunTexture, new Vector2(sunTimeX, sunAngle + Main.sunModY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, SunTexture.Width, SunTexture.Height)), 
                    color6, sunRotation, new Vector2(SunTexture.Width / 2, SunTexture.Height / 2), sunScale, SpriteEffects.None, 0f);
            }
        }

        public override float GetCloudAlpha() {
            return 1f - Intensity;
        }

        public override void Activate(Vector2 position, params object[] args) {
            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args) {
            Active = false;
        }

        public override void Reset() {
            Active = false;
        }

        public override bool IsActive() {
            return Active || Intensity > 0.001f;
        }
    }
}