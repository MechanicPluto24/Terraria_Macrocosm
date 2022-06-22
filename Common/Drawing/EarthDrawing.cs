using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Drawing {
    public sealed class EarthDrawing {
        private static Mod MacrocosmMod => ModContent.GetInstance<Macrocosm>();
        public static void InitializeDetour() {
            On.Terraria.Main.DrawSurfaceBG_BackMountainsStep1 += Main_DrawBG;
        }

        private static void Main_DrawBG(On.Terraria.Main.orig_DrawSurfaceBG_BackMountainsStep1 orig, Main self, double backgroundTopMagicNumber, float bgGlobalScaleMultiplier, int pushBGTopHack) {
            orig(self, backgroundTopMagicNumber, bgGlobalScaleMultiplier, pushBGTopHack);
            if (SubworldSystem.IsActive<Moon>()) {
                var earthTexture = ModContent.Request<Texture2D>("Macrocosm/Assets/EarthTransparent").Value;
                var earthAtmo = ModContent.Request<Texture2D>("Macrocosm/Assets/EarthAtmo").Value;
                var sb = Main.spriteBatch;
                
                // yeah that's a lot of local variables - Feldy 
                // behaves weird on world edges (where player is not on screen center)
                // also feels kinda laggy at low X speeds 

                // surface layer dimensions in pixels 
                float moonWidth = Main.maxTilesX * 16; 
                float moonSurfaceLayerHeight = (float)Main.worldSurface * 16f;

                // positions relative to the center origin of the surface layer 
                float playerPositionToCenterX = Main.LocalPlayer.position.X - (moonWidth / 2);
                float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (moonSurfaceLayerHeight / 2);

                // feel free to play around with these :)
                // we could store them in a data structure once we have background celestial bodies 
                float parallax_X = 0.01f; 
                float parallax_Y = 0.03f;
                float averageDrawOffsetAbovePlayerY = 250f; // the offset above screen center while on moon surface vertical center

                Vector2 position = new Vector2((Main.screenWidth  / 2) - playerPositionToCenterX * parallax_X , 
                                               (Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallax_Y - averageDrawOffsetAbovePlayerY);

                Main.spriteBatch.End();                       
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);              
                sb.Draw(earthAtmo, position, null, Color.White, 0f, earthAtmo.Size() / 2, 1f, default, 0f);                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                sb.Draw(earthTexture, position, null, Color.White, 0f, earthTexture.Size() / 2, 1f, default, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            
        }
    }
}