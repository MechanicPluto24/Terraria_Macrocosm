using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class GenericDrawHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_FilterManager.EndCapture += FilterManager_EndCapture;
        }
        public void Unload()
        {
            On_FilterManager.EndCapture -= FilterManager_EndCapture;
        }
        void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D renderTarget2, Color clearColor)
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SparkleDust.DrawAll(sb);
            sb.End();
            orig(self, finalTexture, screenTarget1, renderTarget2, clearColor);
        }
    }
}
