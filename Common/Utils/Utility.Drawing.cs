using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static Vector2 ScreenCenter => new(Main.screenWidth / 2f, Main.screenHeight / 2f);

        public static Vector2 ScreenCenterInWorld => Main.screenPosition + ScreenCenter;

        public static Rectangle ScreenRectangle => new(0, 0, Main.screenWidth, Main.screenHeight);


        /// <summary>
        /// Draw a MagicPixel trail behind a projectile, with length based on the trail cache length  
        /// </summary>
        /// <param name="rotatableOffsetFromCenter"> offset from projectile center when rotation is 0 </param>
        /// <param name="startWidth"> The trail width near the projectile </param>
        /// <param name="endWidth"> The trail width at its end </param>
        /// <param name="startColor"> The trail color near the projectile  </param>
        /// <param name="endColor"> The trail color at its end </param>
        public static void DrawMagicPixelTrail(this Projectile proj, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
        {
            DrawMagicPixelTrail(proj.Size / 2f, proj.oldPos, proj.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);
        }

        /// <summary>
        /// Draw a MagicPixel trail behind a NPC, with length based on the trail cache length  
        /// </summary>
        /// <param name="rotatableOffsetFromCenter"> offset from NPC center when rotation is 0 </param>
        /// <param name="startWidth"> The trail width near the NPC </param>
        /// <param name="endWidth"> The trail width at its end </param>
        /// <param name="startColor"> The trail color near the NPC </param>
        /// <param name="endColor"> The trail color at its end </param>
        public static void DrawMagicPixelTrail(this NPC npc, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
        {
            DrawMagicPixelTrail(npc.Size / 2f, npc.oldPos, npc.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);
        }

        /// <summary>
        /// Draw a MagicPixel trail behind a particle, with length based on the trail cache length  
        /// </summary>
        /// <param name="rotatableOffsetFromCenter"> offset from projectile center when rotation is 0 </param>
        /// <param name="startWidth"> The trail width near the projectile </param>
        /// <param name="endWidth"> The trail width at its end </param>
        /// <param name="startColor"> The trail color near the projectile  </param>
        /// <param name="endColor"> The trail color at its end </param>
        public static void DrawMagicPixelTrail(this Particle particle, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
        {
            DrawMagicPixelTrail(particle.Size / 2f, particle.OldPositions, particle.OldRotations, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);
        }

        /// <summary> Adapted from Terraria.Main.DrawTrail </summary>
        public static void DrawMagicPixelTrail(Vector2 origin, Vector2[] oldPos, float[] oldRot, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
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
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, v1 - Main.screenPosition, rect, color, v2.ToRotation() + ((float)Math.PI / 2f), new Vector2(rect.Width / 2f, rect.Height), new Vector2(MathHelper.Lerp(startWidth, endWidth, (float)k / oldPos.Length), v2.Length()), SpriteEffects.None, 1);
                }
            }
        }

        /// <summary>
        /// <paramref name="lineFraming"/> is a delegate found in <see cref="Terraria.Utils"/>
        /// <br/> See example implementations in: 
        /// <br/> <see cref="DelegateMethods.RainbowLaserDraw(int, Vector2, float, Rectangle, out float, out Rectangle, out Vector2, out Color)"/>
        /// <br/> <see cref="DelegateMethods.TurretLaserDraw(int, Vector2, float, Rectangle, out float, out Rectangle, out Vector2, out Color)"/>
        /// <br/> <see cref="DelegateMethods.LightningLaserDraw(int, Vector2, float, Rectangle, out float, out Rectangle, out Vector2, out Color)"/>
        /// </summary>
        public static void DrawBeam(Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor, Terraria.Utils.LaserLineFraming lineFraming)
        {
            DelegateMethods.c_1 = beamColor; // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by the lineFraming delegate.
            Terraria.Utils.DrawLaser(Main.spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
        }

        public static void DrawStar(Vector2 position, int points, Color color, float scale = 1f, float rotation = 0f, SpriteEffects spriteEffects = SpriteEffects.None, bool entity = false)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float rotationStep = MathHelper.Pi / points;

            for (int i = 0; i < points; i++)
            {
                float angle = rotationStep * i;

                if (entity)
                {
                    Main.EntitySpriteDraw(tex, position, null, color, angle + rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(tex, position, null, color, angle + rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
            }
        }

        public static void DrawStar(Vector2 position, List<float> rotations, Color color, float scale = 1f, SpriteEffects spriteEffects = SpriteEffects.None, bool entity = false)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            foreach (float rotation in rotations)
            {
                if (entity)
                {
                    Main.EntitySpriteDraw(tex, position, null, color, rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(tex, position, null, color, rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
            }
        }

        public static void DrawStar(Vector2 position, int points, Color color, Vector2 scale, float rotation = 0f, SpriteEffects spriteEffects = SpriteEffects.None, bool entity = false)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float rotationStep = MathHelper.Pi / points;

            for (int i = 0; i < points; i++)
            {
                float angle = rotationStep * i;

                if (entity)
                {
                    Main.EntitySpriteDraw(tex, position, null, color, angle + rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(tex, position, null, color, angle + rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
            }
        }

        public static void DrawStar(Vector2 position, List<float> rotations, Color color, Vector2 scale, SpriteEffects spriteEffects = SpriteEffects.None, bool entity = false)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            foreach (float rotation in rotations)
            {
                if (entity)
                {
                    Main.EntitySpriteDraw(tex, position, null, color, rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(tex, position, null, color, rotation, tex.Size() / 2f, scale, spriteEffects, 0f);
                }
            }
        }

        public static void DrawSwingEffectStar(float opacity, SpriteEffects dir, Vector2 drawpos, Microsoft.Xna.Framework.Color drawColor, Microsoft.Xna.Framework.Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            float fade = InverseLerp(fadeInStart, fadeInEnd, flareCounter, clamped: true) * InverseLerp(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);

            Color color = (shineColor * opacity * 0.5f).WithOpacity(0f) * fade;
            Color color2 = drawColor * 0.5f * fade;

            Vector2 scaleX = new Vector2(fatness.X * 0.5f, scale.X) * fade;
            _ = new Vector2(fatness.Y * 0.5f, scale.Y) * fade;

            DrawStar(drawpos, 2, color, scaleX, spriteEffects: dir, entity: true);
            DrawStar(drawpos, 2, color2, scaleX * 0.6f, spriteEffects: dir, entity: true);
        }

        public static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D sparkleTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Color bigColor = shineColor * opacity * 0.5f;
            bigColor.A = 0;
            Vector2 origin = sparkleTexture.Size() / 2f;
            Color smallColor = drawColor * 0.5f;
            float lerpValue = Terraria.Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Terraria.Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
            Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
            bigColor *= lerpValue;
            smallColor *= lerpValue;
            // Bright, large part
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
            // Dim, small part
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
        }

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


        /// <summary> Used for linear scaling along an entire day/night cycle </summary>
        public static float ScaleNoonToMidnight(float min, float max)
        {
            float dayLength = (float)MacrocosmSubworld.CurrentDayLength;
            float nightLength = (float)MacrocosmSubworld.CurrentNightLength;
            float totalCycleLength = dayLength + nightLength;

            double totalTime = Main.dayTime ? Main.time : dayLength + Main.time;
            double currentTime = totalTime % totalCycleLength;

            float middle = (max + min) / 2f;
            float amplitude = (max - min) / 2f;
            float brightness;

            if (currentTime <= dayLength)
            {
                brightness = middle + amplitude * (float)Math.Sin((currentTime / dayLength) * Math.PI);
            }
            else
            {
                double nightTime = currentTime - dayLength;
                brightness = middle + amplitude * (float)Math.Sin((nightTime / nightLength) * Math.PI + Math.PI);
            }

            return brightness;
        }

        public static Vector2 DrawString(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black.WithAlpha(baseColor.A), rotation, origin, baseScale, maxWidth, spread);
            return ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White.WithAlpha(baseColor.A), rotation, origin, baseScale, out _, maxWidth);
        }


        #region BaseMod BaseDrawing

        //public static ShaderContext shaderContext = new ShaderContext();
        //------------------------------------------------------//
        //----------------BASE DRAWING CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods for various drawing functions, such //
        // as guns, afterimages, drawing on the player, etc.    //
        //------------------------------------------------------//
        //  Author(s): Grox the Great, Yoraiz0r                 //
        //------------------------------------------------------//

        public static void DrawInvasionProgressBar(SpriteBatch sb, int progress, int progressMax, bool forceDisplay, ref int displayCount, ref float displayAlpha, Texture2D iconTex, string displayText, string percentText = null, Color backgroundColor = default, Vector2 offset = default)
        {
            if (Main.invasionProgressMode == 2 && forceDisplay && displayCount < 160)
            {
                displayCount = 160;
            }
            if (!Main.gamePaused && displayCount > 0)
            {
                displayCount = Math.Max(0, displayCount - 1);
            }

            if (displayCount > 0) { displayAlpha += 0.05f; } else { displayAlpha -= 0.05f; }
            if (displayAlpha < 0f)
            {
                displayAlpha = 0f;
            }

            if (displayAlpha > 1f)
            {
                displayAlpha = 1f;
            }

            if (displayAlpha <= 0f)
            {
                return;
            }

            float displayScalar = 0.5f + (displayAlpha * 0.5f);

            int displayWidth = (int)(200f * displayScalar);
            int displayHeight = (int)(45f * displayScalar);
            Vector2 basePosition = new Vector2(Main.screenWidth - 120, Main.screenHeight - 40) + offset;
            Rectangle displayRect = new((int)basePosition.X - (displayWidth / 2), (int)basePosition.Y - (displayHeight / 2), displayWidth, displayHeight);
            Terraria.Utils.DrawInvBG(Main.spriteBatch, displayRect, new Color(63, 65, 151, 255) * 0.785f);
            string displayText2 = progressMax == 0 ? progress.ToString() : ((int)(progress * 100f / progressMax)).ToString() + "%";
            if (percentText != null)
            {
                displayText2 = percentText;
            }
            //displayText2 = Language.GetTextValue("Game.WaveCleared", displayText2);
            Texture2D barTex = TextureAssets.ColorBar.Value;
            if (progressMax != 0)
            {
                Main.spriteBatch.Draw(barTex, basePosition, null, Color.White * displayAlpha, 0f, new Vector2(barTex.Width / 2, 0f), displayScalar, SpriteEffects.None, 0f);
                float progressPercent = MathHelper.Clamp(progress / (float)progressMax, 0f, 1f);
                float scalarX = 169f * displayScalar;
                float scalarY = 8f * displayScalar;
                Vector2 vector4 = basePosition + (Vector2.UnitY * scalarY) + (Vector2.UnitX * 1f);
                _ = Terraria.Utils.DrawBorderString(Main.spriteBatch, displayText2, vector4, Microsoft.Xna.Framework.Color.White * displayAlpha, displayScalar, 0.5f, 1f, -1);
                vector4 += Vector2.UnitX * (progressPercent - 0.5f) * scalarX;
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Microsoft.Xna.Framework.Color(255, 241, 51) * displayAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(scalarX * progressPercent, scalarY), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Microsoft.Xna.Framework.Color(255, 165, 0, 127) * displayAlpha, 0f, new Vector2(1f, 0.5f), new Vector2(2f, scalarY), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, vector4, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), Microsoft.Xna.Framework.Color.Black * displayAlpha, 0f, new Vector2(0f, 0.5f), new Vector2(scalarX * (1f - progressPercent), scalarY), SpriteEffects.None, 0f);
            }

            Vector2 center = new Vector2(Main.screenWidth - 120, Main.screenHeight - 80) + offset;
            Vector2 stringLength = FontAssets.ItemStack.Value.MeasureString(displayText);
            Microsoft.Xna.Framework.Rectangle textRect = Terraria.Utils.CenteredRectangle(center, (stringLength + new Vector2(iconTex.Width + 20, 10f)) * displayScalar);
            Terraria.Utils.DrawInvBG(Main.spriteBatch, textRect, backgroundColor);
            Main.spriteBatch.Draw(iconTex, textRect.Left() + (Vector2.UnitX * displayScalar * 8f), null, Microsoft.Xna.Framework.Color.White * displayAlpha, 0f, new Vector2(0f, iconTex.Height / 2), displayScalar * 0.8f, SpriteEffects.None, 0f);
            _ = Terraria.Utils.DrawBorderString(Main.spriteBatch, displayText, textRect.Right() + (Vector2.UnitX * displayScalar * -8f), Microsoft.Xna.Framework.Color.White * displayAlpha, displayScalar * 0.9f, 1f, 0.4f, -1);
        }


        //NOTE: HIGHLY UNSTABLE, ONLY USE IF YOU KNOW WHAT YOU ARE DOING!	
        public static Texture2D StitchTogetherTileTex(Texture2D tex, int tileType, int width = -1, int[] heights = null)
        {
            TileObjectData data = TileObjectData.GetTileData(tileType, 0);
            if (width == -1)
            {
                width = data.CoordinateWidth;
            }

            heights ??= data.CoordinateHeights; int padding = data.CoordinatePadding;
            List<Texture2D> subTexs = new();
            //List<Texture2D> subTexs2 = new List<Texture2D>();		
            for (int w = 0; w < data.Width; w++)
            {
                //subTexs.Clear();
                for (int h = 0; h < data.Height; h++)
                {
                    int currentHeight = 0, tempH = h;
                    while (tempH > 0) { currentHeight += heights[tempH] + padding; tempH--; }
                    subTexs.Add(GetCroppedTex(tex, new Rectangle(w * (width + padding), currentHeight, width, heights[h])));
                }
                /*for(int m = 0; m < subTexs.Count; m++)
				{
					int currentHeight = 0, int tempH = (data.Height - 1);
					while(tempH > 0){ currentHeight += heights[tempH];  tempH--; }	
					Rectangle newBounds = new Rectangle(0, 0, data.Width * width, newHeight);
					Texture2D tex = new Texture2D(Main.instance.GraphicsDevice, newBounds);
				}*/
            }
            int newHeight = 0, tempH2 = data.Height - 1;
            while (tempH2 > 0) { newHeight += heights[tempH2]; tempH2--; }
            Rectangle newBounds = new(0, 0, data.Width * width, newHeight);
            Texture2D tex2 = new(Main.instance.GraphicsDevice, newBounds.Width, newBounds.Height);
            List<Vector2> drawPos = new();
            for (int m = 0; m < subTexs.Count; m++)
            {
                drawPos.Add(new Vector2(width * m, 0));
            }

            return DrawTextureToTexture(tex2, subTexs.ToArray(), drawPos.ToArray());
        }

        //NOTE: HIGHLY UNSTABLE, ONLY USE IF YOU KNOW WHAT YOU ARE DOING!
        public static Texture2D DrawTextureToTexture(Texture2D toDrawTo, Texture2D[] toDraws, Vector2[] drawPos)
        {
            RenderTarget2D renderTarget = new(Main.instance.GraphicsDevice, toDrawTo.Width, toDrawTo.Height, false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            Main.instance.GraphicsDevice.SetRenderTarget(renderTarget);
            Main.instance.GraphicsDevice.Clear(Color.Black);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int m = 0; m < toDraws.Length; m++)
            {
                Texture2D toDraw = toDraws[m];
                DrawTexture(Main.spriteBatch, toDraw, 0, drawPos[m], toDraw.Width, toDraw.Height, 1f, 0f, 0, 1, toDraw.Bounds, null);
            }
            Main.spriteBatch.End();
            Main.instance.GraphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        public static Texture2D GetCroppedTex(Texture2D texture, Rectangle rect)
        {
            return GetCroppedTex(texture, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Texture2D GetCroppedTex(Texture2D texture, int startX, int startY, int newWidth, int newHeight)
        {
            Rectangle newBounds = texture.Bounds;
            newBounds.X += startX;
            newBounds.Y += startY;
            newBounds.Width = newWidth;
            newBounds.Height = newHeight;
            Texture2D croppedTexture = new(Main.instance.GraphicsDevice, newBounds.Width, newBounds.Height);
            // Copy the data from the cropped region into a buffer, then into the new texture
            Color[] data = new Color[newBounds.Width * newBounds.Height];
            texture.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
            croppedTexture.SetData(data);
            return croppedTexture;
        }


        /*
         * Returns a rectangle representing the frame on a texture, can offset on the x axis.
         * 
         * pixelSpaceX/pixelSpaceY : The 'pixel space' seperating two frames in the texture on the X/Y axis, respectively.
         */
        public static Rectangle GetAdvancedFrame(int currentFrame, int frameOffsetX, int frameWidth, int frameHeight, int pixelSpaceX = 0, int pixelSpaceY = 2)
        {
            int column = currentFrame / frameOffsetX;
            currentFrame -= column * frameOffsetX;
            pixelSpaceY *= currentFrame;
            int startX = frameOffsetX == 0 ? 0 : column * (frameWidth + pixelSpaceX);
            int startY = (frameHeight * currentFrame) + pixelSpaceY;
            return new Rectangle(startX, startY, frameWidth, frameHeight);
        }

        /*
         * Returns a rectangle representing the frame on a texture.
         * 
         * pixelSpaceX/pixelSpaceY : The 'pixel space' seperating two frames in the texture on the X/Y axis, respectively.
         */
        public static Rectangle GetFrame(int currentFrame, int frameWidth, int frameHeight, int pixelSpaceX = 0, int pixelSpaceY = 2)
        {
            pixelSpaceY *= currentFrame;
            int startY = (frameHeight * currentFrame) + pixelSpaceY;
            return new Rectangle(0, startY, frameWidth - pixelSpaceX, frameHeight);
        }


        ///<summary>
        /// Returns a color roughly associated with the given dye. (special dyes return null)
        ///</summary>
        public static Color? GetDyeColor(int dye)
        {
            Color? returnColor = null;
            float brightness = 1f;
            if (dye is >= 13 and <= 24) { brightness = 0.7f; dye -= 12; } //black
            if (dye is >= 45 and <= 56) { brightness = 1.3f; dye -= 44; } //silver
            if (dye is >= 32 and <= 43) { brightness = 1.5f; dye -= 31; } //bright dyes
            switch (dye)
            {
                case 1: returnColor = new Color(248, 63, 63); break; //red
                case 2: returnColor = new Color(248, 148, 63); break; //orange
                case 3: returnColor = new Color(248, 242, 62); break; //yellow
                case 4: returnColor = new Color(157, 248, 70); break; //lime
                case 5: returnColor = new Color(48, 248, 70); break; //green
                case 6: returnColor = new Color(60, 248, 70); break; //teal
                case 7: returnColor = new Color(62, 242, 248); break; //cyan
                case 8: returnColor = new Color(64, 181, 247); break; //sky blue
                case 9: returnColor = new Color(66, 95, 247); break; //blue
                case 10: returnColor = new Color(159, 65, 247); break; //purple
                case 11: returnColor = new Color(212, 65, 247); break; //violet
                case 12: returnColor = new Color(242, 63, 131); break; //pink
                case 31: returnColor = new Color(226, 226, 226); break; //silver
                case 44: returnColor = new Color(40, 40, 40); break; //black
                case 62: returnColor = new Color(157, 248, 70); break; //yellow gradient dye
                case 63: returnColor = new Color(64, 181, 247); break; //cyan gradient dye
                case 64: returnColor = new Color(212, 65, 247); break; //violet gradient dye
            }
            if (returnColor != null && brightness != 1f)
            {
                returnColor = Utility.ColorMult((Color)returnColor, brightness);
            }

            return returnColor;
        }

        ///<summary>
        /// Returns a color associated with the given vanilla gem type.
        ///</summary>
        public static Color GetGemColor(int type)
        {
            if (type == 181) { return Color.MediumOrchid; }
            else //Amethyst
            if (type == 180) { return Color.Gold; }
            else //Topaz
            if (type == 177) { return Color.DeepSkyBlue; }
            else //Sapphire
            if (type == 178) { return Color.Crimson; }
            else //Ruby
            if (type == 179) { return Color.LimeGreen; }
            else //Emerald
            if (type == 182) { return Color.GhostWhite; }
            else //Diamond
            if (type == 999) { return Color.Orange; } //Amber
            return Color.Black;
        }

        ///<summary>
        /// Returns true if the requirements for drawing the player's held item are satisfied.
        ///</summary>
        public static bool ShouldDrawHeldItem(Player drawPlayer)
        {
            return ShouldDrawHeldItem(drawPlayer.inventory[drawPlayer.selectedItem], drawPlayer.itemAnimation, drawPlayer.wet, drawPlayer.dead);
        }

        /*
         * Returns true if the requirements for drawing the held item are satisfied.
         * 
         * isDead : should be false, is for players mostly.
         */
        public static bool ShouldDrawHeldItem(Item item, int itemAnimation, bool isWet, bool isDead = false)
        {
            return (itemAnimation > 0 || item.holdStyle > 0) && item.type > ItemID.None && !isDead && !item.noUseGraphic && (!isWet || !item.noWet);
        }

        /*
         * Draw a weapon in a sword-like fashion. (ie only when used, centered and rotating)
         * 
         * Returns : the value for LetDraw.
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the sword's position on the X/Y axis.
         */
        public static bool DrawHeldSword(object sb, int shader, Player drawPlayer, Color lightColor = default, float scale = 0f, float xOffset = 0, float yOffset = 0, Rectangle? frame = null, int frameCount = 1, Texture2D overrideTex = null)
        {
            if (ShouldDrawHeldItem(drawPlayer))
            {
                Item item = drawPlayer.inventory[drawPlayer.selectedItem];
                DrawHeldSword(sb, overrideTex ?? TextureAssets.Item[item.type].Value, shader, drawPlayer.itemLocation, item, drawPlayer.direction, drawPlayer.itemRotation, scale <= 0f ? item.scale : scale, lightColor, item.color, xOffset, yOffset, drawPlayer.gravDir, drawPlayer, frame, frameCount);
                return false;
            }
            return true;
        }

        /*
         * Draw a texture in a sword-like fashion. (ie only when used, centered and rotating)
         *
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the sword's position on the X/Y axis.
         */
        public static void DrawHeldSword(object sb, Texture2D tex, int shader, Vector2 position, Item item, int direction, float itemRotation, float itemScale, Color lightColor = default, Color wepColor = default, float xOffset = 0, float yOffset = 0, float gravDir = -1f, Entity entity = null, Rectangle? frame = null, int frameCount = 1)
        {
            if (frame == null) { frame = new Rectangle(0, 0, tex.Width, tex.Height); }
            if (lightColor == default) { lightColor = GetLightColor(position); }
            xOffset *= direction;
            SpriteEffects spriteEffect = direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (gravDir == -1f) { yOffset *= -1; spriteEffect |= SpriteEffects.FlipVertically; }
            if (entity is Player drawPlayer)
            {
                yOffset -= drawPlayer.gfxOffY;
            }
            else
            if (entity is NPC drawNPC)
            {
                yOffset -= drawNPC.gfxOffY;
            }
            int drawType = item.type;

            Vector2 drawPos = position - Main.screenPosition;
            Vector2 texOrigin = new(tex.Width * 0.5f, tex.Height * 0.5f / frameCount);
            Vector2 rotOrigin = new Vector2(texOrigin.X - (texOrigin.X * direction), gravDir == -1f ? 0 : tex.Height) + new Vector2(xOffset, -yOffset);

            if (gravDir == -1f) //reverse gravity
            {
                if (sb is List<DrawData>)
                {
                    DrawData dd = new(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                    {
                        shader = shader
                    };
                    ((List<DrawData>)sb).Add(dd);
                }
                else
                if (sb is SpriteBatch)
                {
                    ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                }

                if (wepColor != default)
                {
                    if (sb is List<DrawData>)
                    {
                        DrawData dd = new(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                        {
                            shader = shader
                        };
                        ((List<DrawData>)sb).Add(dd);
                    }
                    else
                    if (sb is SpriteBatch)
                    {
                        ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                    }
                }
            }
            else //normal gravity
            {
                if (drawType is 425 or 507)
                {
                    spriteEffect = direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                }
                if (sb is List<DrawData>)
                {
                    DrawData dd = new(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                    {
                        shader = shader
                    };
                    ((List<DrawData>)sb).Add(dd);
                }
                else
                if (sb is SpriteBatch)
                {
                    ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                }

                if (wepColor != default)
                {
                    if (sb is List<DrawData>)
                    {
                        DrawData dd = new(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                        {
                            shader = shader
                        };
                        ((List<DrawData>)sb).Add(dd);
                    }
                    else
                    if (sb is SpriteBatch)
                    {
                        ((SpriteBatch)sb).Draw(tex, drawPos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                    }
                }
            }
        }


        public static bool DrawHeldGun(object sb, int shader, Player drawPlayer, Color lightColor = default, float scale = 0f, float xOffset = 0, float yOffset = 0, bool shakeX = false, bool shakeY = false, float shakeScalarX = 1.0f, float shakeScalarY = 1.0f, Rectangle? frame = null, int frameCount = 1, Texture2D overrideTex = null)
        {
            if (ShouldDrawHeldItem(drawPlayer))
            {
                Item item = drawPlayer.inventory[drawPlayer.selectedItem];
                DrawHeldGun(sb, overrideTex ?? TextureAssets.Item[item.type].Value, shader, drawPlayer.itemLocation, item, drawPlayer.direction, drawPlayer.itemRotation, scale <= 0f ? item.scale : scale, lightColor, item.color, xOffset, yOffset, shakeX, shakeY, shakeScalarX, shakeScalarY, drawPlayer.gravDir, drawPlayer, frame, frameCount);
                return false;
            }
            return true;
        }

        /*
         * Draws a texture in a gun-like fashion. (ie only when used and in the direction of the cursor)
         * 
         * direction : the direction the sprite should point. (-1 for left, 1 for right)
         * itemRotation : Rotation of the item.
         * itemScale : Scale of the item.
         * lightColor : color of the light the weapon is at.
         * wepColor : weapon's tint.
         * XOffset / YOffset : Offsets the gun's position on the X/Y axis.
         * shakeX / shakeY : If true, shakes the sprite on the X/Y axis.
         * shakeScaleX / shakeScaleY : If shakeX/shakeY is true, this scales the amount it shakes by.
         * gravDir : the direction of gravity.
         * entity : If drawing for a player or npc, the instance of them. (can be null)
         */
        public static void DrawHeldGun(object sb, Texture2D tex, int shader, Vector2 position, Item item, int direction, float itemRotation, float itemScale, Color lightColor = default, Color wepColor = default, float xOffset = 0, float yOffset = 0, bool shakeX = false, bool shakeY = false, float shakeScalarX = 1.0f, float shakeScalarY = 1.0f, float gravDir = 1f, Entity entity = null, Rectangle? frame = null, int frameCount = 1)
        {
            if (frame == null) { frame = new Rectangle(0, 0, tex.Width, tex.Height); }
            if (lightColor == default) { lightColor = GetLightColor(position); }
            SpriteEffects spriteEffect = direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (gravDir == -1f) { yOffset *= -1; spriteEffect |= SpriteEffects.FlipVertically; }
            int type = item.type;
            int fakeType = type;
            Vector2 texOrigin = new(tex.Width / 2, (float)(tex.Height / 2) / frameCount);
            if (entity is Player drawPlayer)
            {
                yOffset += drawPlayer.gfxOffY;
            }
            else
            if (entity is NPC drawNPC)
            {
                yOffset += drawNPC.gfxOffY;
            }
            Vector2 rotOrigin = new(-(float)xOffset, ((float)(tex.Height / 2) / frameCount) - yOffset);
            if (direction == -1)
            {
                rotOrigin = new Vector2((float)(tex.Width + xOffset), ((float)(tex.Height / 2) / frameCount) - yOffset);
            }
            Vector2 pos = new((int)(position.X - Main.screenPosition.X + texOrigin.X), (int)(position.Y - Main.screenPosition.Y + texOrigin.Y));

            if (shakeX) { pos.X += shakeScalarX * (Main.rand.Next(-5, 6) / 9f); }
            if (shakeY) { pos.Y += shakeScalarY * (Main.rand.Next(-5, 6) / 9f); }

            if (sb is List<DrawData>)
            {
                DrawData dd = new(tex, pos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                {
                    shader = shader
                };
                ((List<DrawData>)sb).Add(dd);
            }
            else
            if (sb is SpriteBatch)
            {
                ((SpriteBatch)sb).Draw(tex, pos, frame, item.GetAlpha(lightColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
            }

            if (wepColor != default)
            {
                if (sb is List<DrawData>)
                {
                    DrawData dd = new(tex, pos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0)
                    {
                        shader = shader
                    };
                    ((List<DrawData>)sb).Add(dd);
                }
                else
                if (sb is SpriteBatch)
                {
                    ((SpriteBatch)sb).Draw(tex, pos, frame, item.GetColor(wepColor), itemRotation, rotOrigin, itemScale, spriteEffect, 0);
                }
            }
            try { if (type != fakeType) { item.type = type; } }
            catch { }
        }

        ///<summary>
        /// Draws the given texture in a spear-like fashion (texture is oriented at the upper-right corner) using the projectile provided.
        ///</summary>
        public static void DrawProjectileSpear(object sb, Texture2D texture, int shader, Projectile p, Color? overrideColor = null, float offsetX = 0f, float offsetY = 0f)
        {
            offsetX += -texture.Width * 0.5f;
            Color lightColor = overrideColor != null ? (Color)overrideColor : p.GetAlpha(GetLightColor(Main.player[p.owner].Center));
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            offsetY -= Main.player[p.owner].gfxOffY;
            Vector2 offset = Utility.RotateVector(p.Center, p.Center + new Vector2(p.direction == -1 ? offsetX : offsetY, p.direction == 1 ? offsetX : offsetY), p.rotation - 2.355f) - p.Center;
            if (sb is List<DrawData>)
            {
                DrawData dd = new(texture, p.Center - Main.screenPosition + offset, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, p.rotation, origin, p.scale, p.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
                {
                    shader = shader
                };
                ((List<DrawData>)sb).Add(dd);
            }
            else
            if (sb is SpriteBatch)
            {
                ((SpriteBatch)sb).Draw(texture, p.Center - Main.screenPosition + offset, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, p.rotation, origin, p.scale, p.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public static void DrawAura(object sb, Texture2D texture, int shader, Entity codable, float auraPercent, float distanceScalar = 1f, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
        {
            int frameCount = codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1;
            Rectangle frame = codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Height, texture.Width);
            float scale = codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale;
            float rotation = codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation;
            int spriteDirection = codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection;
            float offsetY2 = codable is NPC ? ((NPC)codable).gfxOffY : 0f;
            DrawAura(sb, texture, shader, codable.position + new Vector2(0f, offsetY2), codable.width, codable.height, auraPercent, distanceScalar, scale, rotation, spriteDirection, frameCount, frame, offsetX, offsetY, overrideColor);
        }

        public static void DrawAura(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float auraPercent, float distanceScalar = 1f, float scale = 1f, float rotation = 0f, int direction = 0, int framecount = 1, Rectangle frame = default, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
        {
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            float percentHalf = auraPercent * 5f * distanceScalar;
            float percentLight = MathHelper.Lerp(0.8f, 0.2f, auraPercent);
            lightColor.R = (byte)(lightColor.R * percentLight);
            lightColor.G = (byte)(lightColor.G * percentLight);
            lightColor.B = (byte)(lightColor.B * percentLight);
            lightColor.A = (byte)(lightColor.A * percentLight);
            Vector2 position2 = position;
            for (int m = 0; m < 4; m++)
            {
                float offX = offsetX;
                float offY = offsetY;
                switch (m)
                {
                    case 0: offX += percentHalf; break;
                    case 1: offX -= percentHalf; break;
                    case 2: offY += percentHalf; break;
                    case 3: offY -= percentHalf; break;
                }
                position2 = new Vector2(position.X + offX, position.Y + offY);
                DrawTexture(sb, texture, shader, position2, width, height, scale, rotation, direction, framecount, frame, lightColor);
            }
        }

        public static void DrawYoyoLine(SpriteBatch sb, Projectile projectile, Texture2D overrideTex = null, Color? overrideColor = null)
        {
            DrawYoyoLine(sb, projectile, Main.player[projectile.owner], projectile.Center, Main.player[projectile.owner].MountedCenter, overrideTex, overrideColor);
        }

        public static void DrawYoyoLine(SpriteBatch sb, Projectile projectile, Entity owner, Vector2 yoyoLoc, Vector2 connectionLoc, Texture2D overrideTex = null, Color? overrideColor = null)
        {
            Vector2 mountedCenter = connectionLoc;
            if (owner is Player)
            {
                mountedCenter.Y += Main.player[projectile.owner].gfxOffY;
            }

            float centerDistX = yoyoLoc.X - mountedCenter.X;
            float centerDistY = yoyoLoc.Y - mountedCenter.Y;
            _ = Math.Sqrt((double)((centerDistX * centerDistX) + (centerDistY * centerDistY)));
            _ = (float)Math.Atan2((double)centerDistY, (double)centerDistX) - 1.57f;
            if (owner is Player && !projectile.counterweight)
            {
                int projDir = -1;
                if (projectile.position.X + projectile.width / 2 < Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2)
                {
                    projDir = 1;
                }

                projDir *= -1;
                ((Player)owner).itemRotation = (float)Math.Atan2((double)(centerDistY * projDir), (double)(centerDistX * projDir));
            }
            bool flag = true;
            if (centerDistX == 0f && centerDistY == 0f) { flag = false; }
            else
            {
                float sqrtCenter = (float)Math.Sqrt((double)((centerDistX * centerDistX) + (centerDistY * centerDistY)));
                sqrtCenter = 12f / sqrtCenter;
                centerDistX *= sqrtCenter;
                centerDistY *= sqrtCenter;
                mountedCenter.X -= centerDistX * 0.1f;
                mountedCenter.Y -= centerDistY * 0.1f;
                centerDistX = yoyoLoc.X - mountedCenter.X;
                centerDistY = yoyoLoc.Y - mountedCenter.Y;
            }
            while (flag)
            {
                float textureHeight = 12f;
                float sqrtCenter = (float)Math.Sqrt((double)((centerDistX * centerDistX) + (centerDistY * centerDistY)));
                float sqrtCenter2 = sqrtCenter;
                if (float.IsNaN(sqrtCenter) || float.IsNaN(sqrtCenter2)) { flag = false; }
                else
                {
                    if (sqrtCenter < 20f) { textureHeight = sqrtCenter - 8f; flag = false; }
                    sqrtCenter = 12f / sqrtCenter;
                    centerDistX *= sqrtCenter;
                    centerDistY *= sqrtCenter;
                    mountedCenter.X += centerDistX;
                    mountedCenter.Y += centerDistY;
                    centerDistX = yoyoLoc.X - mountedCenter.X;
                    centerDistY = yoyoLoc.Y - mountedCenter.Y;
                    if (sqrtCenter2 > 12f)
                    {
                        float scalar = 0.3f;
                        float velocityAverage = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
                        if (velocityAverage > 16f)
                        {
                            velocityAverage = 16f;
                        }

                        velocityAverage = 1f - (velocityAverage / 16f);
                        scalar *= velocityAverage;
                        velocityAverage = sqrtCenter2 / 80f;
                        if (velocityAverage > 1f)
                        {
                            velocityAverage = 1f;
                        }

                        scalar *= velocityAverage;
                        if (scalar < 0f)
                        {
                            scalar = 0f;
                        }

                        scalar *= velocityAverage;
                        scalar *= 0.5f;
                        if (centerDistY > 0f)
                        {
                            centerDistY *= 1f + scalar;
                            centerDistX *= 1f - scalar;
                        }
                        else
                        {
                            velocityAverage = Math.Abs(projectile.velocity.X) / 3f;
                            if (velocityAverage > 1f)
                            {
                                velocityAverage = 1f;
                            }

                            velocityAverage -= 0.5f;
                            scalar *= velocityAverage;
                            if (scalar > 0f)
                            {
                                scalar *= 2f;
                            }

                            centerDistY *= 1f + scalar;
                            centerDistX *= 1f - scalar;
                        }
                    }
                    float rotation = (float)Math.Atan2((double)centerDistY, (double)centerDistX) - 1.57f;
                    int stringColor = Main.player[projectile.owner].stringColor;
                    Color color = overrideColor != null && stringColor <= 0 ? (Color)overrideColor : WorldGen.paintColor(stringColor);
                    if (color.R < 75)
                    {
                        color.R = 75;
                    }

                    if (color.G < 75)
                    {
                        color.G = 75;
                    }

                    if (color.B < 75)
                    {
                        color.B = 75;
                    }

                    if (stringColor == 13) { color = new Color(20, 20, 20); }
                    else if (stringColor is 14 or 0) { color = new Color(200, 200, 200); }
                    else if (stringColor == 28) { color = new Color(163, 116, 91); }
                    else if (stringColor == 27) { color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB); }
                    color.A = (byte)(color.A * 0.4f);
                    float colorScalar = 0.5f;
                    if (overrideColor == null)
                    {
                        color = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f), color);
                        color = new Microsoft.Xna.Framework.Color((byte)(color.R * colorScalar), (byte)(color.G * colorScalar), (byte)(color.B * colorScalar), (byte)(color.A * colorScalar));
                    }
                    Texture2D tex = overrideTex ?? TextureAssets.FishingLine.Value;
                    Vector2 texCenter = new(tex.Width * 0.5f, tex.Height * 0.5f);
                    Main.spriteBatch.Draw(TextureAssets.FishingLine.Value, new Vector2(mountedCenter.X - Main.screenPosition.X + texCenter.X, mountedCenter.Y - Main.screenPosition.Y + texCenter.Y) - new Vector2(6f, 0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, (int)textureHeight)), color, rotation, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
                }
            }
        }

        ///<summary>
        /// Draws a fishing line from the given projectile bobber to the player owning it.
        ///</summary>
        public static void DrawFishingLine(SpriteBatch sb, Projectile projectile, Vector2 rodLoc, Vector2 bobberLoc, Texture2D overrideTex = null, Color? overrideColor = null)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.bobber && Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].holdStyle > 0)
            {
                float mountedCenterX = player.MountedCenter.X;
                float mountedCenterY = player.MountedCenter.Y;
                mountedCenterY += Main.player[projectile.owner].gfxOffY;
                _ = Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type;
                float gravDir = Main.player[projectile.owner].gravDir;

                mountedCenterX += (float)(rodLoc.X * Main.player[projectile.owner].direction);
                if (Main.player[projectile.owner].direction < 0)
                {
                    mountedCenterX -= 13f;
                }

                mountedCenterY -= rodLoc.Y * gravDir;

                if (gravDir == -1f)
                {
                    mountedCenterY -= 12f;
                }

                Vector2 mountedCenter = new(mountedCenterX, mountedCenterY);
                mountedCenter = Main.player[projectile.owner].RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
                float projLineCenterX = projectile.position.X + (projectile.width * 0.5f) - mountedCenter.X;
                float projLineCenterY = projectile.position.Y + (projectile.height * 0.5f) - mountedCenter.Y;
                projLineCenterX += bobberLoc.X; projLineCenterY += bobberLoc.Y;
                _ = Math.Sqrt((double)((projLineCenterX * projLineCenterX) + (projLineCenterY * projLineCenterY)));
                _ = (float)Math.Atan2((double)projLineCenterY, (double)projLineCenterX) - 1.57f;
                bool flag2 = true;
                if (projLineCenterX == 0f && projLineCenterY == 0f) { flag2 = false; }
                else
                {
                    float num15 = (float)Math.Sqrt((double)((projLineCenterX * projLineCenterX) + (projLineCenterY * projLineCenterY)));
                    num15 = 12f / num15;
                    projLineCenterX *= num15;
                    projLineCenterY *= num15;
                    mountedCenter.X -= projLineCenterX;
                    mountedCenter.Y -= projLineCenterY;
                    projLineCenterX = projectile.position.X + (projectile.width * 0.5f) - mountedCenter.X;
                    projLineCenterY = projectile.position.Y + (projectile.height * 0.5f) - mountedCenter.Y;
                }
                while (flag2)
                {
                    float num16 = 12f;
                    float num17 = (float)Math.Sqrt((double)((projLineCenterX * projLineCenterX) + (projLineCenterY * projLineCenterY)));
                    float num18 = num17;
                    if (float.IsNaN(num17) || float.IsNaN(num18)) { flag2 = false; }
                    else
                    {
                        if (num17 < 20f)
                        {
                            num16 = num17 - 8f;
                            flag2 = false;
                        }
                        num17 = 12f / num17;
                        projLineCenterX *= num17;
                        projLineCenterY *= num17;
                        mountedCenter.X += projLineCenterX;
                        mountedCenter.Y += projLineCenterY;
                        projLineCenterX = projectile.position.X + (projectile.width * 0.5f) - mountedCenter.X;
                        projLineCenterY = projectile.position.Y + (projectile.height * 0.1f) - mountedCenter.Y;
                        if (num18 > 12f)
                        {
                            float num19 = 0.3f;
                            float num20 = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
                            if (num20 > 16f)
                            {
                                num20 = 16f;
                            }

                            num20 = 1f - (num20 / 16f);
                            num19 *= num20;
                            num20 = num18 / 80f;
                            if (num20 > 1f)
                            {
                                num20 = 1f;
                            }

                            num19 *= num20;
                            if (num19 < 0f)
                            {
                                num19 = 0f;
                            }

                            num20 = 1f - (projectile.localAI[0] / 100f);
                            num19 *= num20;
                            if (projLineCenterY > 0f)
                            {
                                projLineCenterY *= 1f + num19;
                                projLineCenterX *= 1f - num19;
                            }
                            else
                            {
                                num20 = Math.Abs(projectile.velocity.X) / 3f;
                                if (num20 > 1f)
                                {
                                    num20 = 1f;
                                }

                                num20 -= 0.5f;
                                num19 *= num20;
                                if (num19 > 0f)
                                {
                                    num19 *= 2f;
                                }

                                projLineCenterY *= 1f + num19;
                                projLineCenterX *= 1f - num19;
                            }
                        }
                        float rotation2 = (float)Math.Atan2((double)projLineCenterY, (double)projLineCenterX) - 1.57f;
                        Color color2 = Lighting.GetColor((int)mountedCenter.X / 16, (int)(mountedCenter.Y / 16f), overrideColor != null ? (Color)overrideColor : new Color(200, 200, 200, 100));
                        Texture2D tex = overrideTex ?? TextureAssets.FishingLine.Value;
                        Vector2 texCenter = new(tex.Width * 0.5f, tex.Height * 0.5f);
                        sb.Draw(tex, new Vector2(mountedCenter.X - Main.screenPosition.X + (texCenter.X * 0.5f), mountedCenter.Y - Main.screenPosition.Y + (texCenter.Y * 0.5f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, tex.Width, (int)num16)), color2, rotation2, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        /*
         * Draws the given texture multiple times with each one being farther away and more faded depending on velocity.
         * Uses a Entity(NPC/Projectile) for width, height, position, rotation, sprite direction, and velocity. If an npc, also uses framecount and frame.
         */
        public static void DrawAfterimage(object sb, Texture2D texture, int shader, Entity codable, float distanceScalar = 1.0F, float sizeScalar = 1.0f, int imageCount = 7, bool useOldPos = true, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null, Rectangle? overrideFrame = null, int overrideFrameCount = 0)
        {
            int frameCount = overrideFrameCount > 0 ? overrideFrameCount : codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1;
            Rectangle frame = overrideFrame != null ? (Rectangle)overrideFrame : codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale;
            float rotation = codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation;
            int spriteDirection = codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection;
            Vector2[] velocities = [codable.velocity];
            if (useOldPos)
            {
                velocities = codable is NPC ? ((NPC)codable).oldPos : ((Projectile)codable).oldPos;
            }
            float offsetY2 = codable is NPC ? ((NPC)codable).gfxOffY : 0f;
            DrawAfterimage(sb, texture, shader, codable.position + new Vector2(0f, offsetY2), codable.width, codable.height, velocities, scale, rotation, spriteDirection, frameCount, frame, distanceScalar, sizeScalar, imageCount, useOldPos, offsetX, offsetY, overrideColor);
        }

        /*
         * Draws the given texture multiple times with each one being farther away and more faded depending on velocity.
         * 
         * oldPoints : an array of points used to draw the afterimage.
         * distanceScalar : How far away from each other each image is.
         * sizeScalar : the amount to scale by for each image. (NOTE: this is ADDITIVE!)
         * fullbright : If the images are fullbright or not.
         * alphaAmt : The amount of alpha to subtract with each image. (0-255)
         * imageCount : How many images to draw.
         * useOldPos : If true, considers the given array as old positions instead of old oldPoints.
         */
        public static void DrawAfterimage(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, Vector2[] oldPoints, float scale = 1f, float rotation = 0f, int direction = 0, int framecount = 1, Rectangle frame = default, float distanceScalar = 1.0F, float sizeScalar = 1f, int imageCount = 7, bool useOldPos = true, float offsetX = 0f, float offsetY = 0f, Color? overrideColor = null)
        {
            _ = new Vector2(texture.Width / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            Vector2 velAddon = default;
            Vector2 originalpos = position;
            Vector2 offset = new(offsetX, offsetY);
            for (int m = 1; m <= imageCount; m++)
            {
                scale *= sizeScalar;
                Color newLightColor = lightColor;
                newLightColor.R = (byte)(newLightColor.R * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.G = (byte)(newLightColor.G * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.B = (byte)(newLightColor.B * (imageCount + 3 - m) / (imageCount + 9));
                newLightColor.A = (byte)(newLightColor.A * (imageCount + 3 - m) / (imageCount + 9));
                if (useOldPos)
                {
                    position = Vector2.Lerp(originalpos, m - 1 >= oldPoints.Length ? oldPoints[^1] : oldPoints[m - 1], distanceScalar);
                    DrawTexture(sb, texture, shader, position + offset, width, height, scale, rotation, direction, framecount, frame, newLightColor);
                }
                else
                {
                    Vector2 velocity = m - 1 >= oldPoints.Length ? oldPoints[^1] : oldPoints[m - 1];
                    velAddon += velocity * distanceScalar;
                    DrawTexture(sb, texture, shader, position + offset - velAddon, width, height, scale, rotation, direction, framecount, frame, newLightColor);
                }
            }
        }

        public static void DrawChain(object sb, Texture2D texture, int shader, Vector2 start, Vector2 end, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
            DrawChain(sb, [texture, texture, texture], shader, start, end, Jump, overrideColor, scale, drawEndsUnder, OnDrawTex);
        }

        //code written by Yoraiz0r, heavily edited by GroxTheGreat
        /*
         * Draws a chain from the start position to the end position using the texture provided.
         * 
         * textures : an array of 3 textures: the 'start' texture, the segment texture and the 'end' texture.
         * start : the starting point of the chain.
         * end : the ending point of the chain.
         * Jump : The amount to 'jump' to draw the next piece of chain. If -1, will use the texture height.
         * overrideColor : the color to draw the chain with.
         * scale : the scalar of the chain.
         * drawEndsUnder : If true, the end textures will be drawn under the segment texture. Otherwise, drawn above it.
         * OnDrawTex : If not null, called when the chain draws a texture. Return true to draw the chain piece, false to not draw it. Parameters, in order:
         *             1 - The texture.
         *             2 - The world position of the chain.
         *             3 - The draw position of the chain.
         *             4 - The center of the texture.
         *             5 - The frame of the texture being used.
         *             6 - The color the texture is being drawn.
         *             7 - The rotation of the chain.
         *             8 - The scale of the chain.
         *             9 - The count of this chain piece in the entire thing. (-1 for start tex, -2 for end tex)
         */
        public static void DrawChain(object sb, Texture2D[] textures, int shader, Vector2 start, Vector2 end, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
            if (Jump <= 0f) { Jump = (textures[1].Height - 2f) * scale; }
            Vector2 dir = end - start;
            dir.Normalize();
            float length = Vector2.Distance(start, end);
            float Way = 0f;
            float rotation = Utility.RotationTo(start, end) - 1.57f;
            int texID = 0;
            int maxTextures = textures.Length - 2;
            int currentChain = 0;
            while (Way < length)
            {
                float texWidth;
                float texHeight;
                Vector2 texCenter;
                Vector2 v;
                Color lightColor;
                void drawEnds()
                {
                    if (textures[0] != null && Way == 0f)
                    {
                        float texWidth2 = textures[0].Width;
                        float texHeight2 = textures[0].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = start - Main.screenPosition + texCenter2;
                        Color lightColor2 = overrideColor != null ? (Color)overrideColor : GetLightColor(start + texCenter2);
                        if (OnDrawTex != null && !OnDrawTex(textures[0], start + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -1)) { }
                        else
                        {
                            if (sb is List<DrawData>)
                            {
                                DrawData dd = new(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0)
                                {
                                    shader = shader
                                };
                                ((List<DrawData>)sb).Add(dd);
                            }
                            else
                            if (sb is SpriteBatch)
                            {
                                ((SpriteBatch)sb).Draw(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                    if (textures[maxTextures + 1] != null && Way + Jump >= length)
                    {
                        float texWidth2 = textures[maxTextures + 1].Width;
                        float texHeight2 = textures[maxTextures + 1].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = end - Main.screenPosition + texCenter2;
                        Color lightColor2 = overrideColor != null ? (Color)overrideColor : GetLightColor(end + texCenter2);
                        if (OnDrawTex != null && !OnDrawTex(textures[maxTextures + 1], end + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -2)) { }
                        else
                        {
                            if (sb is List<DrawData>)
                            {
                                DrawData dd = new(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0)
                                {
                                    shader = shader
                                };
                                ((List<DrawData>)sb).Add(dd);
                            }
                            else
                            if (sb is SpriteBatch)
                            {
                                ((SpriteBatch)sb).Draw(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                }
                texWidth = textures[1].Width;
                texHeight = textures[1].Height;
                texCenter = new Vector2(texWidth / 2f, texHeight / 2f) * scale;

                v = start + (dir * Way) + texCenter;
                if (InDrawZone(v))
                {
                    v -= Main.screenPosition;
                    if ((Way == 0f || Way + Jump >= length) && drawEndsUnder) { drawEnds(); }
                    lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(start + (dir * Way) + texCenter);
                    texID++;
                    if (texID >= maxTextures) { texID = 0; }
                    if (OnDrawTex != null && !OnDrawTex(textures[texID + 1], start + (dir * Way) + texCenter, v - texCenter, texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, scale, currentChain)) { }
                    else
                    {
                        if (sb is List<DrawData>)
                        {
                            DrawData dd = new(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0)
                            {
                                shader = shader
                            };
                            ((List<DrawData>)sb).Add(dd);
                        }
                        else
                        if (sb is SpriteBatch)
                        {
                            ((SpriteBatch)sb).Draw(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
                        }
                    }
                    currentChain++;
                    if ((Way == 0f || Way + Jump >= length) && !drawEndsUnder) { drawEnds(); }
                }
                Way += Jump;
            }
        }

        public static void DrawVectorChain(object sb, Texture2D[] textures, int shader, Vector2[] chain, float Jump = 0f, Color? overrideColor = null, float scale = 1f, bool drawEndsUnder = false, Func<Texture2D, Vector2, Vector2, Vector2, Rectangle, Color, float, float, int, bool> OnDrawTex = null)
        {
            if (Jump <= 0f) { Jump = (textures[1].Height - 2f) * scale; }

            float length = 0f;
            for (int m = 0; m < chain.Length - 1; m++)
            {
                length += Vector2.Distance(chain[m], chain[m + 1]);
            }
            Vector2 start = chain[0];
            Vector2 end = chain[^1];
            Vector2 dir = end - start;
            dir.Normalize();
            float Way = 0f;
            float rotation = Utility.RotationTo(chain[0], chain[1]) - 1.57f;
            int texID = 0;
            int maxTextures = textures.Length - 2;
            int currentChain = 0;
            _ = chain[0];
            while (Way < length)
            {
                float texWidth;
                float texHeight;
                Vector2 texCenter;
                Vector2 v;
                Color lightColor;
                void drawEnds()
                {
                    if (textures[0] != null && Way == 0f)
                    {
                        float texWidth2 = textures[0].Width;
                        float texHeight2 = textures[0].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = start - Main.screenPosition + texCenter2;
                        Color lightColor2 = overrideColor != null ? (Color)overrideColor : GetLightColor(start + texCenter2);
                        if (OnDrawTex != null && !OnDrawTex(textures[0], start + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -1)) { }
                        else
                        {
                            if (sb is List<DrawData>)
                            {
                                DrawData dd = new(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0)
                                {
                                    shader = shader
                                };
                                ((List<DrawData>)sb).Add(dd);
                            }
                            else
                            if (sb is SpriteBatch spriteBatch)
                            {
                                spriteBatch.Draw(textures[0], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                    if (textures[maxTextures + 1] != null && Way + Jump >= length)
                    {
                        float texWidth2 = textures[maxTextures + 1].Width;
                        float texHeight2 = textures[maxTextures + 1].Height;
                        Vector2 texCenter2 = new Vector2(texWidth2 / 2f, texHeight2 / 2f) * scale;
                        Vector2 v2 = end - Main.screenPosition + texCenter2;
                        Color lightColor2 = overrideColor != null ? (Color)overrideColor : GetLightColor(end + texCenter2);
                        if (OnDrawTex != null && !OnDrawTex(textures[maxTextures + 1], end + texCenter2, v2 - texCenter2, texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, scale, -2)) { }
                        else
                        {
                            if (sb is List<DrawData>)
                            {
                                DrawData dd = new(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0)
                                {
                                    shader = shader
                                };
                                ((List<DrawData>)sb).Add(dd);
                            }
                            else if (sb is SpriteBatch spriteBatch)
                            {
                                spriteBatch.Draw(textures[maxTextures + 1], v2 - texCenter2, new Rectangle(0, 0, (int)texWidth2, (int)texHeight2), lightColor2, rotation, texCenter2, scale, SpriteEffects.None, 0);
                            }
                        }
                    }
                }
                texWidth = textures[1].Width;
                texHeight = textures[1].Height;
                texCenter = new Vector2(texWidth / 2f, texHeight / 2f) * scale;

                v = MultiLerpVector(Way / length, chain) + texCenter;
                Vector2 nextV = MultiLerpVector(Math.Max(length - 1, Way + 1) / length, chain) + texCenter;
                if (v != nextV)
                {
                    rotation = RotationTo(v, nextV) - 1.57f;
                }

                if (InDrawZone(v))
                {
                    v -= Main.screenPosition;
                    if ((Way == 0f || Way + Jump >= length) && drawEndsUnder) { drawEnds(); }
                    lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(start + (dir * Way) + texCenter);
                    texID++;
                    if (texID >= maxTextures) { texID = 0; }
                    if (OnDrawTex != null && !OnDrawTex(textures[texID + 1], start + (dir * Way) + texCenter, v - texCenter, texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, scale, currentChain)) { }
                    else
                    {
                        if (sb is List<DrawData>)
                        {
                            DrawData dd = new(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0)
                            {
                                shader = shader
                            };
                            ((List<DrawData>)sb).Add(dd);
                        }
                        else
                        if (sb is SpriteBatch)
                        {
                            ((SpriteBatch)sb).Draw(textures[texID + 1], v - texCenter, new Rectangle(0, 0, (int)texWidth, (int)texHeight), lightColor, rotation, texCenter, scale, SpriteEffects.None, 0);
                        }
                    }
                    currentChain++;
                    if ((Way == 0f || Way + Jump >= length) && !drawEndsUnder) { drawEnds(); }
                }
                Way += Jump;
            }
        }



        /*
         * Draws the given texture using the override color.
         * Uses a Entity for width, height, position, rotation, and sprite direction.
         */
        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            DrawTexture(sb, texture, shader, codable, 1, overrideColor, drawCentered, overrideOrigin);
        }

        /*
         * Draws the given texture using the override color.
         * Uses a Entity for width, height, position, rotation, and sprite direction.
         */
        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, int framecountX, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            Color lightColor = overrideColor != null ? (Color)overrideColor : codable is Item ? ((Item)codable).GetAlpha(GetLightColor(codable.Center)) : codable is NPC ? GetLightColor(codable.Center) : codable is Projectile ? ((Projectile)codable).GetAlpha(GetLightColor(codable.Center)) : GetLightColor(codable.Center);
            int frameCount = codable is Item ? 1 : codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1;
            Rectangle frame = codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = codable is Item ? ((Item)codable).scale : codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale;
            float rotation = codable is Item ? 0 : codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation;
            int spriteDirection = codable is Item ? 1 : codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection;
            float offsetY = codable is NPC ? ((NPC)codable).gfxOffY : 0f;
            DrawTexture(sb, texture, shader, codable.position + new Vector2(0f, offsetY), codable.width, codable.height, scale, rotation, spriteDirection, frameCount, framecountX, frame, lightColor, drawCentered, overrideOrigin);
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, Rectangle frame, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            DrawTexture(sb, texture, shader, position, width, height, scale, rotation, direction, framecount, 1, frame, overrideColor, drawCentered, overrideOrigin);
        }

        ///<summary>
        /// Draws the given texture using lighting nearby, or the overriden color given.
        ///</summary>
        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, int framecountX, Rectangle frame, Color? overrideColor = null, bool drawCentered = false, Vector2 overrideOrigin = default)
        {
            Vector2 origin = overrideOrigin != default ? overrideOrigin : new Vector2(frame.Width / framecountX / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            if (sb is List<DrawData> drawDatas)
            {
                DrawData dd = new(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, frame, framecount, framecountX, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
                {
                    shader = shader
                };
                drawDatas.Add(dd);
            }
            else if (sb is SpriteBatch spriteBatch)
            {
                bool applyDye = shader > 0;
                if (applyDye)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                }
                spriteBatch.Draw(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, frame, framecount, framecountX, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                if (applyDye)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
            }
        }

        ///<summary>
        /// Debug draw method, draws a hitbox with absolutes, not taking into account anything else.
        ///</summary>
        public static void DrawHitbox(SpriteBatch sb, Rectangle hitbox, Color? overrideColor = null)
        {
            Vector2 origin = default;
            Color lightColor = overrideColor != null ? (Color)overrideColor : Color.White;
            Vector2 position = new Vector2(hitbox.Left, hitbox.Top) - Main.screenPosition;
            sb.Draw(TextureAssets.MagicPixel.Value, position, hitbox, lightColor, 0f, origin, 1f, SpriteEffects.None, 0);
        }

        ///<summary>
        /// Returns the draw position of a texture for tiles.
        ///</summary>
        public static Vector2 GetTileDrawPosition(int x, int y, int width, int height, Vector2 drawOffset)
        {
            return new Vector2((x * 16) - (int)Main.screenPosition.X - ((width - 16f) / 2f), (y * 16) - (int)Main.screenPosition.Y) + drawOffset;
        }

        ///<summary>
        /// Returns the draw position of a texture for npcs and projectiles.
        ///</summary>
        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, Rectangle frame, int framecount, float scale, bool drawCentered = false)
        {
            return GetDrawPosition(position, origin, width, height, texWidth, texHeight, frame, framecount, 1, scale, drawCentered);
        }

        ///<summary>
        /// Returns the draw position of a texture for npcs and projectiles.
        ///</summary>
        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, Rectangle frame, int framecount, int framecountX, float scale, bool drawCentered = false)
        {
            Vector2 screenPos = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            if (drawCentered)
            {
                Vector2 texHalf = new(texWidth / framecountX / 2, texHeight / framecount / 2);
                return position + new Vector2(width / 2, height / 2) - (texHalf * scale) + (origin * scale) - screenPos;
            }
            return position - screenPos + new Vector2(width / 2, height) - (new Vector2(texWidth / framecountX / 2, texHeight / framecount) * scale) + (origin * scale) + new Vector2(0f, 5f);
        }

        public static float GetYOffset(Player player)
        {
            return GetYOffset(player.bodyFrame, player.gravDir);
        }

        /*
         * Returns an offset for Y that simulates how player frames offset normally. 
         * This allows you to have a one-frame .png file that still 'bobs' up and down even if it doesn't animate.
         */
        public static float GetYOffset(Rectangle frame, float gravDir = 0f)
        {
            int frameID = frame.Y / frame.Height;
            return frameID is 7 or 8 or 9 or 14 or 15 or 16 ? gravDir < 0f ? 2f : -2f : 0f;
        }

        //used by InDrawZone to prevent making a new rectangle every time the method is called
        private static Rectangle drawZoneRect = default;

        public static bool InDrawZone(Vector2 vec, bool noScreenPos = false)
        {
            if ((int)Main.screenPosition.X - 300 != drawZoneRect.X || (int)Main.screenPosition.Y - 300 != drawZoneRect.Y)
            {
                drawZoneRect = new Rectangle((int)Main.screenPosition.X - 300, (int)Main.screenPosition.Y - 300, Main.screenWidth + 600, Main.screenHeight + 600);
            }

            if (noScreenPos)
            {
                vec += Main.screenPosition;
            }

            return drawZoneRect.Contains((int)vec.X, (int)vec.Y);
        }

        public static bool InDrawZone(Rectangle rect)
        {
            if ((int)Main.screenPosition.X - 300 != drawZoneRect.X || (int)Main.screenPosition.Y - 300 != drawZoneRect.Y)
            {
                drawZoneRect = new Rectangle((int)Main.screenPosition.X - 300, (int)Main.screenPosition.Y - 300, Main.screenWidth + 600, Main.screenHeight + 600);
            }

            return drawZoneRect.Intersects(rect);
        }

        #endregion

    }
}