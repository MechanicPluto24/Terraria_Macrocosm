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

namespace Macrocosm.Backgrounds
{
    public class MoonSky : CustomSky
    {
        public bool Active;
        public float Intensity;

        private readonly UnifiedRandom _random = new UnifiedRandom();

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            Vector4 value = inColor.ToVector4();
            return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.5f));
        }

        private readonly Mod mod = ModContent.GetInstance<Macrocosm>();
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SunTexture = ModContent.Request<Texture2D>("Backgrounds/Sun_0").Value;
            Texture2D SkyTex = ModContent.Request<Texture2D>("Backgrounds/MoonSky").Value;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw((Texture2D)TextureAssets.BlackTile, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * Intensity);
                spriteBatch.Draw(SkyTex, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * Intensity));
                float num64 = 1f;
                num64 -= Main.cloudAlpha * 1.5f;
                if (num64 < 0f)
                {
                    num64 = 0f;
                }
                int num20 = (int)(Main.time / 54000.0 * (Main.screenWidth + TextureAssets.Sun.Width() * 2)) - TextureAssets.Sun.Width();
                int num21 = 0;
                float num22 = 1f;
                float rotation = (float)(Main.time / 54000.0) * 2f - 7.3f;
                double bgTop = (-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0;
                if (Main.dayTime)
                {
                    double num26;
                    if (Main.time < 27000.0)
                    {
                        num26 = Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0);
                        num21 = (int)(bgTop + num26 * 250.0 + 180.0);
                    }
                    else
                    {
                        num26 = Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0);
                        num21 = (int)(bgTop + num26 * 250.0 + 180.0);
                    }
                    num22 = (float)(1.2 - num26 * 0.4);
                }
                Color color6 = new Color((byte)(255f * num64), (byte)(Color.White.G * num64), (byte)(Color.White.B * num64), (byte)(255f * num64));
                Main.spriteBatch.Draw(SunTexture, new Vector2(num20, num21 + Main.sunModY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, SunTexture.Width, SunTexture.Height)), color6, rotation, new Vector2(SunTexture.Width / 2, SunTexture.Height / 2), num22, SpriteEffects.None, 0f);
            }
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