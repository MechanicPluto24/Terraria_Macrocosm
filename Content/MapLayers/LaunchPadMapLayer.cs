using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.MapLayers
{
    public class LaunchPadMapLayer : ModMapLayer
    {
        private Asset<Texture2D> texture;
        public override void Load()
        {
            texture = ModContent.Request<Texture2D>(GetType().Namespace.Replace(".", "/") + "/LaunchPadMap");
        }

        public override Position GetDefaultPosition() => new Before(IMapLayer.Pings);

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            foreach (LaunchPad launchPad in LaunchPadManager.GetLaunchPads(MacrocosmSubworld.CurrentID))
            {
                if (context.Draw(texture.Value, launchPad.CenterTile.ToVector2() + new Vector2(0, 1), Color.White, new SpriteFrame(1, 1, 0, 0), 0.95f, 0.95f, Alignment.Bottom).IsMouseOver)
                    text = launchPad.DisplayName;
            }
        }
    }
}