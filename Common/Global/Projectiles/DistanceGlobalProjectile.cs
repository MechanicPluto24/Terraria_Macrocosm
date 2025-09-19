using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles;

public class DistanceGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    private Vector2 initialPosition;
    private int InitalDamage;
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        initialPosition = projectile.Center;
        InitalDamage=projectile.damage;
    }

    public override void PostAI(Projectile projectile)
    {
        Player player = Main.player[projectile.owner];
        if (projectile.DamageType == DamageClass.Ranged)
        {
            bool pointBlank = player.GetModPlayer<ProjectileDistancePlayer>().PointBlank;
            if (pointBlank)
            {
                float mult = Vector2.Distance(initialPosition, projectile.Center) / 300f;
                if (mult > 1f)
                    mult = 1f;
                projectile.damage = (int)(InitalDamage* (1.5f - mult));
            }

            bool zoning = player.GetModPlayer<ProjectileDistancePlayer>().Zoning;
            if (zoning)
            {
                float mult = Vector2.Distance(initialPosition, projectile.Center) / 300f;
                if (mult > 1f)
                    mult = 1f;
                projectile.damage = (int)(InitalDamage * (mult + 0.5f));
        }
    }
}
