using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Debuffs.Radiation;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ReLogic.Content;

namespace  Macrocosm.Content.NPCs.Enemies.Moon.Turrets
{
    public class TurretTaserProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        private static Asset<Texture2D> chainTexture;


        public override void SetDefaults()
        {
         
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        int Timer=0;
        public override void AI()
        {
            NPC OwnerTurret= Main.npc[(int)Projectile.ai[1]];
            if(!OwnerTurret.active || OwnerTurret.type != ModContent.NPCType<TaserTurret>())
                Projectile.Kill();
            TaserTurret Turret = OwnerTurret.ModNPC as TaserTurret; 
            if (Timer<60){
                Projectile.velocity *= 0.95f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else{
                Projectile.velocity = (OwnerTurret.Center+Turret.turretHeight-Projectile.Center).SafeNormalize(Vector2.UnitX) * 10f;
                if(Vector2.Distance(Projectile.Center,OwnerTurret.Center+Turret.turretHeight)<12f)
                    Projectile.Kill();
            }
            Timer++;
            
        }
        public override void OnKill (int timeLeft){
            NPC OwnerTurret= Main.npc[(int)Projectile.ai[1]];
            if(OwnerTurret.active && OwnerTurret.type == ModContent.NPCType<TaserTurret>())
                OwnerTurret.ai[0]=0f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(144,300);
            target.AddBuff(ModContent.BuffType<Paralysis>(),60);
        }
    
        public override bool PreDraw(ref Color lightColor)
        {
            NPC OwnerTurret= Main.npc[(int)Projectile.ai[1]];
            TaserTurret Turret = OwnerTurret.ModNPC as TaserTurret; 

            chainTexture ??= ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/Turrets/TaserChain");
            int chainlength = (int)(Vector2.Distance(Projectile.Center,OwnerTurret.Center+Turret.turretHeight) / (chainTexture.Value).Width);
            for (int i = 0; i <= chainlength; i++)
            {
                Main.EntitySpriteDraw(chainTexture.Value, Projectile.Center +((new Vector2(1,0)).RotatedBy((OwnerTurret.Center+Turret.turretHeight-Projectile.Center).ToRotation())*(i* (chainTexture.Value).Width))-Main.screenPosition, null, lightColor,(OwnerTurret.Center+Turret.turretHeight-Projectile.Center).ToRotation(), chainTexture.Size()/2f, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
}
