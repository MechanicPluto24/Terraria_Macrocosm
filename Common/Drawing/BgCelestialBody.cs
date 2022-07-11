using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Macrocosm.Common.Drawing {
    public static class BgCelestialBody 
    {
        /// <summary>
        /// Draw a parallaxing celestial body with an atmosphere in the background of a world's surface
        /// </summary>
        /// <param name="bodyTexture"> The celestial body to draw </param>
        /// <param name="atmoTexture"> The celestial body's atmosphere </param>
        /// <param name="scale"> The scale of the texture </param>
        /// <param name="averageOffsetX"> The offset from screen center when the player is in the world's horizontal center </param>
        /// <param name="averageOffsetY"> The offset from screen center when the player is midway between world's surface and upper bounds </param>
        /// <param name="parallax_X"> The horizontal parallax speed relative to the player </param>
        /// <param name="parallax_Y"> The vertical parallax speed relative to the player </param>
        public static void Draw(Texture2D bodyTexture, Texture2D atmoTexture, float scale, float averageOffsetX, float averageOffsetY, float parallax_X, float parallax_Y) 
        {               
            // surface layer dimensions in pixels 
            float worldWidth = Main.maxTilesX * 16f; 
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
            float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

            Vector2 position = new Vector2((Main.screenWidth  / 2) - playerPositionToCenterX * parallax_X + averageOffsetX, 
                                            (Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallax_Y + averageOffsetY);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(atmoTexture, position, null, Color.White, 0f, atmoTexture.Size() / 2, scale, default, 0f);
            Main.spriteBatch.End();
            
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(bodyTexture, position, null, Color.White, 0f, bodyTexture.Size() / 2, scale, default, 0f);
        }

        /// <summary>
        /// Draw a parallaxing celestial body in the background of a world's surface
        /// </summary>
        /// <param name="bodyTexture"> The celestial body to draw </param>
        /// <param name="scale"> The scale of the texture </param>
        /// <param name="averageOffsetX"> The offset from screen center when the player is in the world's horizontal center </param>
        /// <param name="averageOffsetY"> The offset from screen center when the player is midway between world's surface and upper bounds </param>
        /// <param name="parallax_X"> The horizontal parallax speed relative to the player </param>
        /// <param name="parallax_Y"> The vertical parallax speed relative to the player </param>
        public static void Draw(Texture2D bodyTexture, float scale, float averageOffsetX, float averageOffsetY, float parallax_X, float parallax_Y)
        {
            // surface layer dimensions in pixels 
            float worldWidth = Main.maxTilesX * 16f;
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
            float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

            Vector2 position = new Vector2((Main.screenWidth / 2) - playerPositionToCenterX * parallax_X + averageOffsetX,
                                            (Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallax_Y + averageOffsetY);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(bodyTexture, position, null, Color.White, 0f, bodyTexture.Size() / 2, scale, default, 0f);
        }
    }
}