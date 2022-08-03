using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Common.Drawing {

    // We could make it non-static if, for some reason, we actually need instances - Feldy 
    public static class CelestialBody {
        /// <summary>
        /// Draws a celestial body with an atmosphere at the screen center, with possible offsets and parallax speeds
        /// </summary>
        /// <param name="bodyTexture"> The celestial body to draw </param>
        /// <param name="atmoTexture"> The celestial body's atmosphere </param>
        /// <param name="scale"> The scale of the texture </param>
        /// <param name="averageOffsetX"> The offset from screen center when the player is in the world's horizontal center </param>
        /// <param name="averageOffsetY"> The offset from screen center when the player is midway between world's surface and upper bounds </param>
        /// <param name="parallax_X"> The horizontal parallax speed relative to the player </param>
        /// <param name="parallax_Y"> The vertical parallax speed relative to the player </param>
        public static void Draw(Texture2D bodyTexture, Texture2D atmoTexture, float scale, float averageOffsetX = 0f, float averageOffsetY = 0f, float parallax_X = 0f, float parallax_Y = 0f) {
            // surface layer dimensions in pixels 
            float worldWidth = Main.maxTilesX * 16f;
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
            float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

            Vector2 position = new Vector2((Main.screenWidth / 2) - playerPositionToCenterX * parallax_X + averageOffsetX,
                                            (Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallax_Y + averageOffsetY);

            Main.spriteBatch.End();

            // draw atmosphere in BlendState.Additive (for proper transparency) and in the EffectMatrix (no scaling with screen size) 
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
            Main.spriteBatch.Draw(atmoTexture, position, null, Color.White, 0f, atmoTexture.Size() / 2, scale, default, 0f);
            Main.spriteBatch.End();

            // draw body in the EffectMatrix 
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
            Main.spriteBatch.Draw(bodyTexture, position, null, Color.White, 0f, bodyTexture.Size() / 2, scale, default, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        /// <summary>
        /// Draws a celestial body at the screen center, with possible offsets and parallax speeds
        /// </summary>
        /// <param name="bodyTexture"> The celestial body to draw </param>
        /// <param name="scale"> The scale of the texture </param>
        /// <param name="averageOffsetX"> The offset from screen center when the player is in the world's horizontal center </param>
        /// <param name="averageOffsetY"> The offset from screen center when the player is midway between world's surface and upper bounds </param>
        /// <param name="parallax_X"> The horizontal parallax speed relative to the player </param>
        /// <param name="parallax_Y"> The vertical parallax speed relative to the player </param>
        public static void Draw(Texture2D bodyTexture, float scale, float averageOffsetX = 0f, float averageOffsetY = 0f, float parallax_X = 0f, float parallax_Y = 0f) {
            // surface layer dimensions in pixels 
            float worldWidth = Main.maxTilesX * 16f;
            float surfaceLayerHeight = (float)Main.worldSurface * 16f;

            // positions relative to the center origin of the surface layer 
            float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
            float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

            Vector2 position = new Vector2((Main.screenWidth / 2) - playerPositionToCenterX * parallax_X + averageOffsetX,
                                            (Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallax_Y + averageOffsetY);

            Main.spriteBatch.End();

            // draw body in the EffectMatrix 
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
            Main.spriteBatch.Draw(bodyTexture, position, null, Color.White, 0f, bodyTexture.Size() / 2, scale, default, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}