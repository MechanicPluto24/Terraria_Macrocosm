using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class DistanceGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private Vector2 initialPosition;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            initialPosition = projectile.Center;
        }

        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.DamageType == DamageClass.Ranged)
            {
                bool pointBlank = player.GetModPlayer<ProjectileDistancePlayer>().PointBlank;
                if (pointBlank)
                {
                    float mult = Vector2.Distance(initialPosition, projectile.Center) / 100f;
                    if (mult > 1f)
                        mult = 1f;
                    projectile.damage = (int)(projectile.damage * (1.5f - mult));
                }

                bool zoning = player.GetModPlayer<ProjectileDistancePlayer>().Zoning;
                if (zoning)
                {
                    float mult = Vector2.Distance(initialPosition, projectile.Center) / 200f;
                    if (mult > 1f)
                        mult = 1f;
                    projectile.damage = (int)(projectile.damage * (mult + 0.5f));
                }
            }
        }
    }
}
