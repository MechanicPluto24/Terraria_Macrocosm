using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    // incomplete
    // TODO Make it stick to walls and add leaping animation.

    public class Leaper : ModNPC
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 39;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 36;
            NPC.height = 44;
            NPC.damage = 65;
            NPC.defense = 60;
            NPC.lifeMax = 900;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.03f;
            NPC.aiStyle = -1;
            AIType = NPCID.ZombieMushroom;
            Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.BannerToItem(Banner);

            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
        }
        private bool PreformingWallAnimation = false;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }

        public bool Jumping = false;

        public static void AILeaper(Leaper leaper, NPC npc, ref float[] ai, Vector2 targetPosition, float accelerationFactor = 0.07f, float velMax = 1f, int maxJumpTilesX = 3, int maxJumpTilesY = 4, bool targetPlayers = true, bool jumpUpPlatforms = false, Action<bool, bool, Vector2, Vector2> onTileCollide = null, bool ignoreJumpTiles = false)
        {
            if (!leaper.PreformingWallAnimation)
            {

                if (targetPosition.X > npc.Center.X)
                    npc.direction = 1;
                else
                    npc.direction = -1;

                //if velocity is less than -1 or greater than 1...
                if (npc.velocity.X < -velMax || npc.velocity.X > velMax)
                {
                    //...and npc is not falling or jumping, slow down x velocity.
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < velMax && npc.direction == 1) //handles movement to the right. Clamps at velMaxX.
                {
                    npc.velocity.X += accelerationFactor;
                    if (npc.velocity.X > velMax)
                    {
                        npc.velocity.X = velMax;
                    }
                }
                else if (npc.velocity.X > -velMax && npc.direction == -1) //handles movement to the left. Clamps at -velMaxX.
                {
                    npc.velocity.X -= accelerationFactor;
                    if (npc.velocity.X < -velMax)
                    {
                        npc.velocity.X = -velMax;
                    }
                }



                Utility.WalkupHalfBricks(npc);

                //if there's a solid floor under us...
                if (Utility.HitTileOnSide(npc, 3))
                {
                    //if the npc's velocity is going in the same direction as the npc's direction...
                    if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                    {
                        //...attempt to jump if needed.
                        Vector2 newVec = Utility.AttemptJump(npc.position, npc.velocity, npc.width, npc.height, npc.direction, targetPosition, npc.directionY, maxJumpTilesX, maxJumpTilesY, velMax, jumpUpPlatforms, ignoreJumpTiles);
                        if (!npc.noTileCollide)
                        {
                            newVec = Collision.TileCollision(npc.position, newVec, npc.width, npc.height);
                            Vector4 slopeVec = Collision.SlopeCollision(npc.position, newVec, npc.width, npc.height);
                            Vector2 slopeVel = new(slopeVec.Z, slopeVec.W);
                            if (onTileCollide != null && npc.velocity != slopeVel)
                            {
                                onTileCollide(npc.velocity.X != slopeVel.X, npc.velocity.Y != slopeVel.Y, npc.velocity, slopeVel);
                            }

                            npc.position = new Vector2(slopeVec.X, slopeVec.Y);
                            npc.velocity = slopeVel;
                        }

                        if (npc.velocity != newVec)
                        {
                            npc.velocity = newVec;
                            npc.netUpdate = true;

                        }
                    }
                }
                if (Utility.HitTileOnSide(npc, 3))
                {
                    leaper.Jumping = false;
                }
                else
                {
                    leaper.Jumping = true;
                }

                if (Utility.HitTileOnSide(npc, 0) || Utility.HitTileOnSide(npc, 1) && (leaper.Jumping == true && leaper.Fear == false && npc.velocity.X == 0))
                {
                    leaper.PreformingWallAnimation = true;
                    npc.frame.Y = 1056;
                }
                else
                    leaper.PreformingWallAnimation = false;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && spawnInfo.Player.ZoneRockLayerHeight ? .1f : 0f;
        }

        public float LightValueFlee = 0.1f; //This light value causes the leaper to flee.
        public float LightValueRage = 0.5f; //This light value causes the leaper to enrage faster.
        public float RageThreshold = 12f; //Determines the switch between fleeing and hostility.
        public float Rage = 0f; //Determines the leapers level of hostility. <5f flee, >5f attack, >7f rage.
        public bool Fear = false; //is it fleeing?
        public float RageManager(float Lightlevel)//Manages the leapers rage.
        {

            if (Lightlevel < LightValueFlee)
                return -0.03f; //Calms down when in darkness
            if (Lightlevel >= LightValueFlee && Lightlevel < LightValueRage)
                return 0.04f; //Becomes angry when in moderate light.
            if (Lightlevel >= LightValueRage)
                return 0.09f; //Becomes enrages quickly while in bright light
            else
                return 0f; //I dont think this will be useful but you never know.
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Rage += RageManager(Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness()); //manage rage.
            //put caps on rage.
            if (Rage > RageThreshold)
                Rage = RageThreshold;
            if (Rage < 0f)
                Rage = 0f;

            if (Rage <= 0.01f)
            {//Pure darkness, stalk the player slowly.
                AILeaper(this, NPC, ref NPC.ai, player.Center, accelerationFactor: 0.08f, velMax: 2f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                Fear = false;
            }
            if (Rage > 0.01f && Rage < RageThreshold)
            {//Flee.
                AILeaper(this, NPC, ref NPC.ai, player.Center, accelerationFactor: -0.07f, velMax: 4f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                Fear = true;
            }
            if (Rage >= RageThreshold)
            {//Attack with increasing speed.
                AILeaper(this, NPC, ref NPC.ai, player.Center, accelerationFactor: 0.2f, velMax: 7f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                Fear = false;
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
        private readonly Range IdleFrames = 0..9;
        private readonly Range JumpingFrames = 10..16;
        private readonly Range RunningFrame = 17..23;
        private readonly Range WallFrame = 24..39;
        private int npcFrame = 0;

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 5;
            int frameIndex = NPC.frame.Y / frameHeight;
            NPC.frameCounter++;
            NPC.spriteDirection = Fear == true ? -NPC.direction : NPC.direction; //Edtited this line to make the leaper face the right direction
            if (PreformingWallAnimation)
            {
                NPC.velocity = Vector2.Zero;
                // Update frame
                if (NPC.frameCounter > ticksPerFrame) 
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (frameIndex >= WallFrame.End.Value)
                {
                    PreformingWallAnimation = false;
                    NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 15f;
                }
                if (Utility.HitTileOnSide(NPC, 0))
                {
                    NPC.spriteDirection = -1;
                }
                if (Utility.HitTileOnSide(NPC, 1))
                {
                    NPC.spriteDirection = 1;
                }
            }
            else
            {
                if (NPC.velocity == Vector2.Zero)
                {
                    // Reset walking 
                    if (!IdleFrames.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * IdleFrames.Start.Value;

                    // Walking animation frame counter, accounting for walk speed
                    NPC.frameCounter += Math.Abs(NPC.velocity.X);

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= IdleFrames.End.Value)
                        NPC.frame.Y = frameHeight * IdleFrames.Start.Value;
                }
                else if (!Jumping)
                {
                    // Reset walking 
                    if (!RunningFrame.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * RunningFrame.Start.Value;

                    // Walking animation frame counter, accounting for walk speed

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= RunningFrame.End.Value)
                        NPC.frame.Y = frameHeight * RunningFrame.Start.Value;
                }
                else if (Jumping)
                {
                    // Reset walking 
                    if (!JumpingFrames.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * JumpingFrames.Start.Value;

                    // Walking animation frame counter, accounting for walk speed

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= JumpingFrames.End.Value)
                        NPC.frame.Y = frameHeight * 16;
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