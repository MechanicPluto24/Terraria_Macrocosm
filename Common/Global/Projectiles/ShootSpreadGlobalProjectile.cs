using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class ShootSpreadGlobalProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            if (player.whoAmI == Main.myPlayer && projectile.DamageType == DamageClass.Ranged && source is EntitySource_ItemUse_WithAmmo)
            {
                float spreadReduction = player.GetModPlayer<MacrocosmPlayer>().ShootSpreadReduction;
                if (spreadReduction > 0f)
                {
                    Vector2 mouseAim = (Main.MouseWorld - projectile.position).SafeNormalize(default);
                    float mouseAngle = mouseAim.ToRotation();
                    float shootAngle = projectile.velocity.ToRotation();
                    float angleDifference = Math.Abs(shootAngle - mouseAngle);

                    if (angleDifference > MathHelper.ToRadians(1f))
                    {
                        Vector2 adjustedVelocity = Vector2.Lerp(projectile.velocity, mouseAim * projectile.velocity.Length(), spreadReduction);

                        float randomAngle = MathHelper.ToRadians(2f * (1f - spreadReduction));
                        adjustedVelocity = adjustedVelocity.RotatedByRandom(randomAngle);
                        projectile.velocity = adjustedVelocity;
                    }
                }
            }
        }

    }
}
