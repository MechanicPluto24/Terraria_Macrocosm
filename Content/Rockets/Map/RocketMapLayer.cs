using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Content.Rockets.Map
{
    public class RocketMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {

            var texture = ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Map/RocketMap", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            foreach (Rocket rocket in RocketManager.Rockets)
            {
                if (context.Draw(texture, rocket.Center / 16f, Color.White, new SpriteFrame(1, 1, 0, 0), 0.95f, 0.95f, Alignment.Bottom).IsMouseOver)
                    text = rocket.DisplayName;
            }
        }
    }
}