using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Subworlds;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianGazeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1000;
        }
        int TimeUntilMandatoryBreak=500;
        int Timer;
        bool Broke=false;
        bool HitSomething=false;
        Vector2 OldVelocity=new Vector2(0,0);
        public override bool OnTileCollide(Vector2 oldVelocity){
            Broke=true;  
            HitSomething=true;  
            OldVelocity=oldVelocity*-1f;
            return false;
        }

        public override void AI(){
            TimeUntilMandatoryBreak--;
            Projectile.velocity.Y+=MacrocosmSubworld.Current.GravityMultiplier;
            if (TimeUntilMandatoryBreak<1){
                Broke=true;
                OldVelocity=Projectile.velocity*-1f;
            }
            if (Broke==true){
                if(HitSomething==true)
                    Projectile.velocity*=0f;
                Timer++;
            }
            if (Timer>30)
                CreateALotOfIce();           
        }
        public void CreateALotOfIce(){
            if (Main.netMode != NetmodeID.MultiplayerClient){
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage/4, 2, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage/4, 2, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage/4, 2, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage/4, 2, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*13f, ModContent.ProjectileType<FrigorianIceShardAlt>(), Projectile.damage/2, 2, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4)*13f, ModContent.ProjectileType<FrigorianIceShardAlt>(), Projectile.damage/2, 2, -1);
    
            }
             Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity*0f, Mod.Find<ModGore>("FrigorianGore1").Type);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity*0f, Mod.Find<ModGore>("FrigorianGore2").Type);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity*0f, Mod.Find<ModGore>("FrigorianGore3").Type);
            for(int i=0; i<3;i++){
            Particle.CreateParticle<IceMist>((p) =>
            {
                p.Position = Projectile.Center;
                p.Velocity = OldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI/4) * 2f;
                p.Scale = 1f;
            }, shouldSync: true
            );
            }
            Projectile.Kill();
        }



    }
    public class FrigorianIceShard : ModProjectile
    {
         public NPC FindClosestNPC(float maxDetectDistance) {//example mod
			NPC closestNPC = null;

			
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

		
			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
			
				if (target.CanBeChasedBy()) {
				
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
        }
    
       
        public override void AI(){
       
        Projectile.rotation=Projectile.velocity.ToRotation();
        NPC closestNPC = FindClosestNPC(9000f);
			if (closestNPC == null){
		
				return;
			}
			else{
			Vector2 vel = (closestNPC.Center-Projectile.Center).SafeNormalize(Vector2.UnitX);
            Projectile.velocity = Projectile.velocity+(vel*0.9f);
			Projectile.velocity =(Projectile.velocity).SafeNormalize(Vector2.UnitX);
			Projectile.velocity *=17f;
			}
            
        }
    }
     public class FrigorianIceShardAlt : ModProjectile
    {
         public NPC FindClosestNPC(float maxDetectDistance) {//example mod
			NPC closestNPC = null;

			
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

		
			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
			
				if (target.CanBeChasedBy()) {
				
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}
        public override void SetDefaults()
        {
            Projectile.scale=0.7f;
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
        }
    

        public override void AI(){
         Projectile.rotation=Projectile.velocity.ToRotation()+MathHelper.ToRadians(90);
         NPC closestNPC = FindClosestNPC(9000f);
			if (closestNPC == null){
		
				return;
			}
			else{
			Vector2 vel = (closestNPC.Center-Projectile.Center).SafeNormalize(Vector2.UnitX);
            Projectile.velocity = Projectile.velocity+(vel*0.5f);
			Projectile.velocity =(Projectile.velocity).SafeNormalize(Vector2.UnitX);
			Projectile.velocity *=13f;
			}
        }
    }
}
