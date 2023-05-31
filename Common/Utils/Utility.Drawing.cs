using System;
using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
		public static Vector2 ScreenCenter => new(Main.screenWidth / 2f, Main.screenHeight / 2f);

		public static void ManipulateColor(ref Color color, byte amount)
        {
            color.R += amount;
            color.G += amount;
            color.B += amount;
        }
        public static void ManipulateColor(ref Color color, float amount)
        {
            color.R *= (byte)Math.Round(color.R * amount);
            color.G += (byte)Math.Round(color.G * amount);
            color.B += (byte)Math.Round(color.B * amount);
        }
        
        /// <summary>
        /// Draw a MagicPixel trail behind a projectile, with length based on the trail cache length  
        /// </summary>
        /// <param name="rotatableOffsetFromCenter"> offset from projectile center when rotation is 0 </param>
        /// <param name="startWidth"> The trail width near the projectile </param>
        /// <param name="endWidth"> The trail width at its end </param>
        /// <param name="startColor"> The trail color near the projectile  </param>
        /// <param name="endColor"> The trail color at its end </param>
        public static void DrawSimpleTrail(this Projectile proj, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
            => DrawSimpleTrail(proj.Size / 2f, proj.oldPos, proj.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);

        /// <summary>
        /// Draw a MagicPixel trail behind a NPC, with length based on the trail cache length  
        /// </summary>
        /// <param name="rotatableOffsetFromCenter"> offset from NPC center when rotation is 0 </param>
        /// <param name="startWidth"> The trail width near the NPC </param>
        /// <param name="endWidth"> The trail width at its end </param>
        /// <param name="startColor"> The trail color near the NPC </param>
        /// <param name="endColor"> The trail color at its end </param>
        public static void DrawSimpleTrail(this NPC npc, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
            => DrawSimpleTrail(npc.Size / 2f, npc.oldPos, npc.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);

		/// <summary> Adapted from Terraria.Main.DrawTrail </summary>
		public static void DrawSimpleTrail(Vector2 origin, Vector2[] oldPos, float[] oldRot, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
        {
            Rectangle rect = new(0, 0, 1, 1);
            for (int k = oldPos.Length - 1; k > 0; k--)
            {
                if (!(oldPos[k] == Vector2.Zero))
                {
                    Vector2 v1 = oldPos[k] + origin + rotatableOffsetFromCenter.RotatedBy(oldRot[k]);
                    Vector2 v2 = oldPos[k - 1] + origin + rotatableOffsetFromCenter.RotatedBy(oldRot[k - 1]) - v1;
                    float brightness = Terraria.Utils.Remap(k, 0f, oldPos.Length, 1f, 0f);
                    Color color = endColor is null ? startColor * brightness : Color.Lerp((Color)endColor, startColor, brightness);

                    SpriteBatch spriteBatch = Main.spriteBatch;
                    SpriteBatchState state = spriteBatch.SaveState();
                    spriteBatch.EndIfBeginCalled();
                    spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, state);
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, v1 - Main.screenPosition, rect, color, v2.ToRotation() + (float)Math.PI / 2f, new Vector2(rect.Width / 2f, rect.Height), new Vector2(MathHelper.Lerp(startWidth, endWidth, (float)k / oldPos.Length), v2.Length()), SpriteEffects.None, 1);
                    spriteBatch.Restore(state);
                }
            }
        }

        /// <summary> Gets the perceived luminance of a color using the NTSC standard as a normalized value </summary>
        public static float GetLuminance(this Color rgbColor)
            => 0.299f * rgbColor.R / 255 + 0.587f * rgbColor.G / 255 + 0.114f * rgbColor.B / 255;


        /// <summary> Gets the perceived luminance of a color using the NTSC standard as a byte </summary>
        public static byte GetLuminance_Byte(this Color rgbColor) => (byte)(rgbColor.GetLuminance() * 255);


        /// <summary> Returns the RGB grayscale of a color using the NTSC standard </summary>
        public static Color ToGrayscale(this Color rgbColor)
        {
            Color result = new();
            result.R = result.G = result.B = rgbColor.GetLuminance_Byte();
            result.A = rgbColor.A;
            return result;
        }

        public static Color NewAlpha(this Color color, float alpha)
            => new(color.R, color.G, color.B, (byte)(alpha * 255));

        public static Color NewAlpha(this Color color, byte alpha)
            => new(color.R, color.G, color.B, alpha);
        

		/// <summary> Convenience method for getting lighting color using an npc or projectile position.</summary>
		public static Color GetLightColor(Vector2 position)
		{
			return Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));
		}

		/// <summary> Convenience method for adding lighting using an npc or projectile position, using a Color instance for color. </summary>
		public static void AddLight(Vector2 position, Color color, float brightnessDivider = 1F)
		{
			AddLight(position, color.R / 255F, color.G / 255F, color.B / 255F, brightnessDivider);
		}


		/// <summary> Convenience method for adding lighting using an npc or projectile position with 0f - 1f color values. </summary>
		public static void AddLight(Vector2 position, float colorR, float colorG, float colorB, float brightnessDivider = 1f)
		{
			Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), colorR / brightnessDivider, colorG / brightnessDivider, colorB / brightnessDivider);
		}


		/// <summary> Returns a premultiplied copy of a texture </summary>
		public static Texture2D ToPremultiplied(this Texture2D texture)
        {
            Texture2D newTexture = new(texture.GraphicsDevice, texture.Width, texture.Height);

            Main.QueueMainThreadAction(() =>
            {
                Color[] buffer = new Color[texture.Width * texture.Height];
                texture.GetData(buffer);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = Color.FromNonPremultiplied(
                        buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
                }
                newTexture.SetData(buffer);
            });

            return newTexture;
        }

    }
}