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
                
                float moonWidth = Main.maxTilesX * 16;
                float positionToCenter = Main.LocalPlayer.position.X - (moonWidth / 2);
                float parallax = 0.015f;
                Vector2 position = new Vector2((Main.screenWidth / 2) - positionToCenter * parallax , 200);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                //sb.Draw(earthAtmo, new Vector2(Main.screenWidth / 2, 200), null, Color.White, 0f, earthAtmo.Size() / 2, 1f, default, 0f);                
                sb.Draw(earthAtmo, position, null, Color.White, 0f, earthAtmo.Size() / 2, 1f, default, 0f);                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                //sb.Draw(earthTexture, new Vector2(Main.screenWidth / 2, 200), null, Color.White, 0f, earthTexture.Size() / 2, 1f, default, 0f);
                sb.Draw(earthTexture, position, null, Color.White, 0f, earthTexture.Size() / 2, 1f, default, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            
        }
    }
}