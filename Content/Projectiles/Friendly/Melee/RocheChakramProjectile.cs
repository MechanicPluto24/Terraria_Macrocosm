using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework.Graphics;
namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class RocheChakramProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 600;
        }
        float Speed=25f;
        int ExplosionTimer=0;
        public bool ShouldExplode()
        {
            Player player = Main.player[Projectile.owner];
            bool FoundOlder=true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RocheChakramProjectile>()] > 5)
            {
                for(int i=0; i<=1000; i++)
                {
                Projectile projectile = Main.projectile[i];
                if(i!=Projectile.whoAmI)
                    if(projectile.type==ModContent.ProjectileType<RocheChakramProjectile>()&&projectile.owner==Projectile.owner&&projectile.active)
                        if(projectile.timeLeft<Projectile.timeLeft)
                            FoundOlder=false;
                }
                
                return FoundOlder;
            }   
            return false;
        }

        public override void AI()
        {
            if(ShouldExplode()==true)
                ExplosionTimer++;
            Projectile.rotation +=0.65f;
            Projectile.velocity= Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Projectile.velocity*=Speed;
            if (Speed>-35f)
                Speed-=0.2f;

            if (Speed<0f)
            {
                Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.velocity *= -Speed;
                if (Projectile.Distance(Main.player[Projectile.owner].Center) < 50f){
                    Projectile.Kill();
                    Item.NewItem(Projectile.GetSource_FromAI(), position: Main.player[Projectile.owner].Center, ModContent.ItemType<RocheChakram>(),noBroadcast:false,noGrabDelay:true,reverseLookup:true);
                }
            }
            if(ExplosionTimer>30)
                Explode();


        }
        public void Explode()
        {
            for(int i=0; i<6; i++){
                if(Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi)*18f, ModContent.ProjectileType<RocheSpike>(), (int)(Projectile.damage/2.5), 1f, Main.myPlayer, ai0: 0f);
            }
            for (int i = 0; i < (int)15; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>());
                    dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                    dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                    dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                    dust.noGravity = true;
                }
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ProtolithDust>());
                    dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                    dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                    dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                    dust.noGravity = true;
                }

            Projectile.Kill();
        }
      

    }
}