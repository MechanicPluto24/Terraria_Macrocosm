using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tools
{
    public class ChandriumDrillProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SolarFlareDrill);
            Projectile.width = 24;
            Projectile.height = 42;
        }

        public override bool PreAI()
        {
            #region Variables
            Player player = Main.player[Projectile.owner];
            Rectangle hitbox = Projectile.Hitbox;

            float lightMultiplier = 0.35f;
            #endregion

            #region Offset
            if (player.direction == 1)
            {
                DrawOffsetX = -5;
            }
            else if (player.direction == -1)
            {
                DrawOffsetX = 5;
            }
            #endregion

            #region Dust
            if (Main.rand.NextBool(4))
            {
                int swingDust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<ChandriumDust>(), -35 * player.direction, default, default, default, Main.rand.NextFloat(1.25f, 1.35f));
                Main.dust[swingDust].velocity *= 0.05f;
            }
            #endregion

            #region Lighting
            Lighting.AddLight(player.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);
            #endregion

            return true;
        }
    }
}