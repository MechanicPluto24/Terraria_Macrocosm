using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utility {
    public static class PlayerUtils {
        public static Rectangle GetSwungItemHitbox(this Player player) {
            //Found in Player.cs
            Item item = player.HeldItem;
            bool flag21 = false;

            Rectangle hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
            if (!Main.dedServ)
                hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, TextureAssets.Item[item.type].Width(), TextureAssets.Item[item.type].Height());

            hitbox.Width = (int)(hitbox.Width * item.scale);
            hitbox.Height = (int)(hitbox.Height * item.scale);
            if (player.direction == -1)
                hitbox.X -= hitbox.Width;

            if (player.gravDir == 1f)
                hitbox.Y -= hitbox.Height;

            if (item.useStyle == ItemUseStyleID.Swing) {
                if (player.itemAnimation < player.itemAnimationMax * 0.333) {
                    if (player.direction == -1)
                        hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);

                    hitbox.Width = (int)(hitbox.Width * 1.4);
                    hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.1);
                }
                else if (player.itemAnimation >= player.itemAnimationMax * 0.666) {
                    if (player.direction == 1)
                        hitbox.X -= (int)(hitbox.Width * 1.2);

                    hitbox.Width *= 2;
                    hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                    hitbox.Height = (int)(hitbox.Height * 1.4);
                }
            }
            else if (item.useStyle == ItemUseStyleID.Thrust) {
                if (player.itemAnimation > player.itemAnimationMax * 0.666) {
                    flag21 = true;
                }
                else {
                    if (player.direction == -1)
                        hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);

                    hitbox.Width = (int)(hitbox.Width * 1.4);
                    hitbox.Y += (int)(hitbox.Height * 0.6);
                    hitbox.Height = (int)(hitbox.Height * 0.6);
                }
            }

            ItemLoader.UseItemHitbox(item, player, ref hitbox, ref flag21);

            return hitbox;
        }
    }
}
