using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    /// <summary>
    /// Adds a glowmask field to GlobalItems.
    /// Code by Hallam 
    /// </summary>
    public class GlowmaskGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public Asset<Texture2D> Texture { get; set; } = null;
        public Color? Color { get; set; } = new(250, 250, 250);
        public bool DrawGlowInWorld { get; set; } = true;
        public int GlowOffsetY { get; set; } = 0;
        public int GlowOffsetX { get; set; } = 0;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Texture != null && DrawGlowInWorld)
            {
                spriteBatch.Draw
                (
                    Texture.Value,
                    new Vector2
                    (
                        item.position.X - Main.screenPosition.X + item.width * 0.5f,
                        item.position.Y - Main.screenPosition.Y + item.height - Texture.Height() * 0.5f + 2f
                    ),
                    new Rectangle(0, 0, Texture.Width(), Texture.Height()),
                    Color ?? Utility.Colorize(lightColor, alphaColor),
                    rotation,
                    Texture.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
            base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }
    }
}
