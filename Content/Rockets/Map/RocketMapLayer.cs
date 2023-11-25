using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Map
{
    public class RocketMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            var texture = ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Map/RocketMap", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            foreach (Rocket rocket in RocketManager.Rockets)
            {
                if (!rocket.ActiveInCurrentWorld)
                    continue;

                if (context.Draw(texture, (rocket.Center + new Vector2(0, rocket.Bounds.Height / 2f)) / 16f, Color.White, new SpriteFrame(1, 1, 0, 0), 0.95f, 0.95f, Alignment.Bottom).IsMouseOver)
                    text = rocket.DisplayName;
            }
        }
    }
}