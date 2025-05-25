using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class PointBlankPotionGlobalProjectile : GlobalProjectile
    {
        static Vector2 initPos;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            initPos=projectile.Center;
        }
        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.DamageType == DamageClass.Ranged)
            {
                bool PointBlank = player.GetModPlayer<PointBlankPotionPlayer>().PointBlank;
                if(PointBlank){
                    float DamageMult = Vector2.Distance(initPos,projectile.Center)/100f;
                    if(DamageMult>1f)
                        DamageMult=1f;
                    projectile.damage = (int)(projectile.damage *(1.5f-DamageMult));
                }
            }
        }

    }
}
