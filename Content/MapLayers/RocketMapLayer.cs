using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.MapLayers
{
    public class RocketMapLayer : ModMapLayer
    {
        private Asset<Texture2D> texture;
        public override void Load()
        {
            texture = ModContent.Request<Texture2D>(GetType().Namespace.Replace(".", "/") + "/RocketMap");
        }

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            foreach (Rocket rocket in RocketManager.Rockets)
            {
                if (!rocket.ActiveInCurrentWorld)
                    continue;

                if (context.Draw(texture.Value, (rocket.Center + new Vector2(0, rocket.Bounds.Height / 2f)) / 16f, Color.White, new SpriteFrame(1, 1, 0, 0), 0.95f, 0.95f, Alignment.Bottom).IsMouseOver)
                    text = rocket.DisplayName;
            }
        }
    }
}