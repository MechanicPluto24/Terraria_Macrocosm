using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Global
{
    /// <summary>
    /// Adds a glowmask field to GlobalItems.
    /// Code by Hallam 
    /// </summary>
    public class GlowmaskGlobalItem : GlobalItem
    {
        public Texture2D Texture = null;
        public int GlowOffsetY = 0;
        public int GlowOffsetX = 0;
        public override bool InstancePerEntity => true;
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Texture != null)
            {
                spriteBatch.Draw
                (
                    Texture,
                    new Vector2
                    (
                        item.position.X - Main.screenPosition.X + item.width * 0.5f,
                        item.position.Y - Main.screenPosition.Y + item.height - Texture.Height * 0.5f + 2f
                    ),
                    new Rectangle(0, 0, Texture.Width, Texture.Height),
                    Color.White,
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
