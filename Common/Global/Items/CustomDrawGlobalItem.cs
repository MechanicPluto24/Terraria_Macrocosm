using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class CustomDrawGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        [CloneByReference] public Asset<Texture2D> CustomHeldTexture { get; set; } = null;
        [CloneByReference] public Asset<Texture2D> CustomHeldTextureGlowmask { get; set; } = null;
        [CloneByReference] public Asset<Texture2D> Glowmask { get; set; } = null;
        public Color? GlowmaskColor { get; set; } = new(250, 250, 250);

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Glowmask != null)
            {
                spriteBatch.Draw
                (
                    Glowmask.Value,
                    new Vector2
                    (
                        item.position.X - Main.screenPosition.X + item.width * 0.5f,
                        item.position.Y - Main.screenPosition.Y + item.height - Glowmask.Height() * 0.5f + 2f
                    ),
                    new Rectangle(0, 0, Glowmask.Width(), Glowmask.Height()),
                    GlowmaskColor ?? Utility.Colorize(lightColor, alphaColor),
                    rotation,
                    Glowmask.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
            base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }
    }
}
