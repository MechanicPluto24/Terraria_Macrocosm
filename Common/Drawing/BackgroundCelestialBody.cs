using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Macrocosm.Common.Drawing {
    public static class BackgroundCelestialBody 
    {
        public static void Draw(SpriteBatch spriteBatch, Texture2D bodyTexture, Texture2D atmoTexture, float scale, float averageOffsetX, float averageOffsetY, float parallax_X, float parallax_Y) 
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
            spriteBatch.Draw(atmoTexture, position, null, Color.White, 0f, atmoTexture.Size() / 2, scale, default, 0f);                
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(bodyTexture, position, null, Color.White, 0f, bodyTexture.Size() / 2, scale, default, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}