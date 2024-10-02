using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class LuminiteRune : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
                private float originalSpeed;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        bool spawned;
        float rotationSpeed;
        private int targetNPC;
        private NPC TargetNPC => Main.npc[targetNPC];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!spawned)
            {
                rotationSpeed = 0.15f;
                Projectile.rotation += rotationSpeed;
                Projectile.frame=Main.rand.Next(0,4);
                spawned = true;
            }

            Projectile.rotation += rotationSpeed;
            rotationSpeed *= 1f - 0.015f;
            Projectile.Opacity -= 0.003f;

            if (WorldGen.SolidTile(Projectile.Center.ToTileCoordinates()))
            {
                rotationSpeed *= 1f - 0.01f;
                
            }
            if (Projectile.Opacity<0.01f){
                Projectile.Kill();
            }
            originalSpeed = Projectile.velocity.Length();
            float closestDistance = 3000f;
             for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.type != NPCID.TargetDummy)
                        {
                            float distance = Vector2.Distance(Projectile.Center, npc.Center);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                targetNPC = npc.whoAmI;
                            }
                        }
                    }
                    if (TargetNPC is not null && Vector2.Distance(Projectile.Center, TargetNPC.Center) < 3000f && TargetNPC.active && !TargetNPC.friendly && TargetNPC.lifeMax > 5 && !TargetNPC.dontTakeDamage)
                    {
                        Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, 0.02f);
                        Projectile.timeLeft--;
                    }
    
            
        }
        public override void OnKill(int timeLeft){
            for (int i=0; i<5;i++){
            Dust dust = Dust.NewDustDirect(Projectile.Center , Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), Scale: 3);
            dust.velocity = Projectile.velocity.RotatedByRandom(MathHelper.Pi*2)*Main.rand.NextFloat(0.1f,0.5f);
            dust.noLight = false;
            dust.noGravity = true;
            }
        }
        public override Color? 	GetAlpha (Color lightColor)=>new Color(255,255,255,255)*Projectile.Opacity;

       
       
    }
}