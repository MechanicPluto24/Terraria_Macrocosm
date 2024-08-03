using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    // incomplete
    // TODO Make it stick to walls.
    public class Leaper : ModNPC, IMoonEnemy
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 39;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 36;
            NPC.height = 44;
            NPC.damage = 65;
            NPC.defense = 60;
            NPC.lifeMax = 700;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            AIType = NPCID.ZombieMushroom;
            Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.BannerToItem(Banner);

            SpawnModBiomes = [ModContent.GetInstance<UndergroundMoonBiome>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && spawnInfo.Player.ZoneRockLayerHeight ? .1f : 0f;
        }
      
        public float LightValueFlee = 0.1f; //This light value causes the leaper to flee.
        public float LightValueRage = 0.5f; //This light value causes the leaper to enrage faster.
        public float RageThreshold = 60f; //Determines the switch between fleeing and hostility.
        public float Rage=0f; //Determines the leapers level of hostility. <5f flee, >5f attack, >7f rage.
        public bool Fear=false; //is it fleeing?
        public float RageManager(float Lightlevel)//Manages the leapers rage.
        {
        if (Lightlevel<LightValueFlee)
            return -0.03f; //Calms down when in darkness
        if (Lightlevel>=LightValueFlee&&Lightlevel<LightValueRage)
            return 0.04f; //Becomes angry when in moderate light.
        if (Lightlevel>=LightValueRage)
            return 0.09f; //Becomes enrages quickly while in bright light
        else
            return 0f; //I dont think this will be useful but you never know.
        }

        public override void AI()
        {
            Rage+=RageManager(Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness()); //manage rage.
            //put caps on rage.
            if(Rage > 60f)
                Rage = 60f;
            if(Rage < 0f)
                Rage = 0f;


            if (Rage<=0.01f){//Pure darkness, stalk the player slowly.
                Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 2, maxJumpTilesX: 15, maxJumpTilesY: 10, moveInterval: 0.06f);
                Fear=false;
            }
            if (Rage>0.01f&&Rage<RageThreshold){//Flee.
                Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 4, maxJumpTilesX: 15, maxJumpTilesY: 10, moveInterval: -0.075f);
                 Fear=true;
            }
            if (Rage>=RageThreshold){//Attack with increasing speed.
                Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 7, maxJumpTilesX: 18, maxJumpTilesY: 12, moveInterval: 0.08f+(Rage/100f));
                Fear=false;
            }

            NPC.despawnEncouraged = false;

            //if(charge)
            //	 NPC.damage = (int)(NPC.defDamage * 1.35f)
            //else
            NPC.damage = NPC.defDamage;
             
            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.1f;
               
        
           
        }

        // frames 0 - 9: idle 
        // frames 10 - 15: start leap
        // frame 16 - leap while mid-air (mid-vacuum?)
        // frames 17 - 23: run 

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 5;

            int idleFrameInitial = 0;
            int idleFrameCount = 10;

            int leapFrameInitial = 10;
            int leapFrameCount = 7;

            int midAirFrame = 16;

            int runFrameInitial = 17;
            int runFrameCount = 7;

            if (!NPC.IsABestiaryIconDummy)
                if (NPC.velocity.Y < -0.1f ||
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16)].HasTile &&
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 1].HasTile &&
                         !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 2].HasTile)
                {
                    NPC.frame.Y = midAirFrame * frameHeight;
                }

            NPC.frameCounter++;
            NPC.spriteDirection = Fear==true ? -NPC.direction:NPC.direction; //Edtited this line to make the leaper face the right direction

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
            else if(NPC.velocity.Y==0f)
            {
                NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + runFrameInitial) * frameHeight;

                if (NPC.frameCounter >= ticksPerFrame * runFrameCount)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = runFrameInitial * frameHeight;
                }
            }
            else{
                NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + leapFrameInitial) * frameHeight;

                if (NPC.frameCounter >= ticksPerFrame * leapFrameCount)
                {
                    NPC.frameCounter = midAirFrame;
                    NPC.frame.Y = midAirFrame * frameHeight;
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
