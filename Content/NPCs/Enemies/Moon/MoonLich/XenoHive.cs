using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich
{
    //TODO get an actual sprite.

    public class XenoHive : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;

            MoRHelper.AddElement(NPC, MoRHelper.Celestial);
        }

        public override void SetDefaults()
        {
            NPC.width = 85;
            NPC.height = 76;
            NPC.damage = 50;
            NPC.defense = 90;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = -1;
            AIType = NPCID.ZombieMushroom;

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }


        int Timer;
        public override void AI()
        {
            Player target = Main.player[NPC.target];



            if (Timer % 600 > 60)
                Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 7, maxJumpTilesX: 18, maxJumpTilesY: 12, moveInterval: 0.08f);
            else
                NPC.velocity.X *= 0f;
            if (Timer % 600 == 30)
            {
                //This has to be synced right?
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<XenoHornet>(), 0, 0f);


            }




            NPC.despawnEncouraged = false;


            NPC.damage = NPC.defDamage;




            Timer++;
        }

        // frames 0 - 9: idle 
        // frames 10 - 15: start leap
        // frame 16 - leap while mid-air (mid-vacuum?)
        // frames 17 - 23: run 

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 8;

            int idleFrameInitial = 0;
            int idleFrameCount = 2;



            int runFrameInitial = 2;
            int runFrameCount = 4;

            if (!NPC.IsABestiaryIconDummy)
                if (NPC.velocity.Y < -0.1f ||
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16)].HasTile &&
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 1].HasTile &&
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 2].HasTile)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

            NPC.frameCounter++;
            NPC.spriteDirection = NPC.direction;

            if (NPC.velocity == Vector2.Zero)
            {
                if (NPC.frameCounter >= ticksPerFrame)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y >= idleFrameCount * frameHeight - 1)
                        NPC.frame.Y = idleFrameInitial * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + runFrameInitial) * frameHeight;

                if (NPC.frameCounter >= ticksPerFrame * runFrameCount)
                {
                    NPC.frameCounter = runFrameInitial;
                    NPC.frame.Y = runFrameInitial * frameHeight;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

            if (NPC.life > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                    dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f) * hit.HitDirection;
                    dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                return; // don't run on the server

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreHead").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreEye").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreLeg").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreLeg").Type);
            }
        }
    }
}