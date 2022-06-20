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
            On.Terraria.Main.DrawBG += Main_DrawBG;
        }

        private static void Main_DrawBG(On.Terraria.Main.orig_DrawBG orig, Main self) {
            orig(self);
            if (SubworldSystem.IsActive<Moon>()) {
                var earthTexture = ModContent.Request<Texture2D>("Macrocosm/Assets/Earth").Value;
                var sb = Main.spriteBatch;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                sb.Draw(earthTexture, new Vector2(Main.screenWidth / 2, 200), null, Color.White, 0f, earthTexture.Size() / 2, 1f, default, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}