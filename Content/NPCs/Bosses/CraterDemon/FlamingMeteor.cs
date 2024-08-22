using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    //(-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(12f, 16f)
    //Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
    public class FlamingMeteor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.lifeMax = 450;
            NPC.timeLeft=600;
        }

        private float flashTimer;
        private float maxFlashTimer = 5;
        private bool spawned;
        public override bool? DrawHealthBar(byte hbPosition,ref float scale,ref Vector2 position )=>false;
        public override void AI()
        {
            if (!spawned)
            {
                
                NPC.velocity = (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(12f, 16f);
                spawned = true;
            }

            NPC.velocity.Y += 0.2f;
            if (NPC.velocity.Y > 24f)
                NPC.velocity.Y = 24f;

            if (NPC.velocity != Vector2.Zero)
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

            Lighting.AddLight(NPC.Center, new Color(242, 142, 35).ToVector3());

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, -NPC.velocity.X * 0.2f, -NPC.velocity.Y * 0.2f, 127, new Color(255, 255, 255), Main.rand.NextFloat(1.1f, 1.4f));
                dust.noGravity = true;
                dust.noLight = true;
            }

            
            
            flashTimer++;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = -NPC.direction;
            int frameSpeed = 5;

            NPC.frameCounter++;

            if (NPC.frameCounter >= frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
           
            if (NPC.life <= 0)
            {
               NPC.velocity *=-0.6f*(hit.Knockback/2f);
               NPC.velocity = NPC.velocity.RotatedByRandom(MathHelper.Pi/3);
               NPC.life=1;
               NPC.friendly = true;
               NPC.dontTakeDamage = true;
            }
        }

     
        

        
    }
}
