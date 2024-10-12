using Macrocosm.Common.Netcode;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Macrocosm.Content.Projectiles.Friendly.Summon;
namespace Macrocosm.Common.Global.Projectiles
{
    public class DroneProjectileManager : ModSystem
    {
        public override void PreUpdateProjectiles(){
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ModContent.ProjectileType<DroneMinion>() && proj.active)
                {
                float amountLesserThanI=0;
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                Projectile projcheck = Main.projectile[j];
                
                if (projcheck.type == ModContent.ProjectileType<DroneMinion>() && projcheck.active)
                    {
                        if(projcheck.ai[2]>proj.ai[2])
                        {
                            amountLesserThanI+=1f;
                        }
                    }
                }
                proj.ai[1]=amountLesserThanI;
                }
            }
        }
    }
}