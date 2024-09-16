using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich
{
    public class XenoHornet : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 34;
            NPC.height = 34;
            NPC.damage = 80;
            NPC.defense = 30;
            NPC.lifeMax = 200;

            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = -1;


        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }
        public float ChasingVel = 1f;
        public override void AI()//Basic AI
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            ChasingVel += 0.08f;
            if (ChasingVel > 12f)
                ChasingVel = 12f;
            Vector2 Homing = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = ((NPC.velocity + (Homing * 0.8f)).SafeNormalize(Vector2.UnitX)) * ChasingVel;

        }



        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 3;
            NPC.frameCounter++;
            NPC.spriteDirection = NPC.direction;


            if (NPC.frameCounter >= ticksPerFrame)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= 5 * frameHeight - 1)
                    NPC.frame.Y = 1 * frameHeight;
            }


        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {



            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs);
                dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f) * hit.HitDirection;
                dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                dust.noGravity = true;
            }
        }

    }
}
