using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles
{
    public class ZoningPotionProjectile : GlobalProjectile
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
                bool Zoning = player.GetModPlayer<ZoningPotionPlayer>().Zoning;
                if(Zoning){
                    float DamageMult = Vector2.Distance(initPos,projectile.Center)/200f;
                    if(DamageMult>1f)
                        DamageMult=1f;
                    projectile.damage = (int)(projectile.damage *(DamageMult+0.5f));
                }
            }
        }

    }
}
