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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }

        private float lightValueFlee = 0.1f; // This light value causes the leaper to flee.
        private float lightValueRage = 0.5f; // This light value causes the leaper to enrage faster.
        private float rageThreshold = 12f; // Determines the switch between fleeing and hostility.

        // TODO: maybe netsync some of these? 
        private int timeSinceLastJump = 0;
        private float rage = 0f; // Determines the leapers level of hostility. <5f flee, >5f attack, >7f rage.
        private bool fear = false; // is it fleeing?
        private bool jumping = false;
        private bool performingWallAnimation = false;

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && spawnInfo.Player.ZoneRockLayerHeight ? .1f : 0f;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            rage += GetRage(Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness()); // Manage rage

            // Put caps on rage
            if (rage > rageThreshold)
                rage = rageThreshold;

            if (rage < 0f)
                rage = 0f;

            // Pure darkness, stalk the player slowly
            if (rage <= 0.01f)
            {
                LeaperAI(player.Center, accelerationFactor: 0.08f, velMax: 2f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                fear = false;
            }

            // Flee
            if (rage > 0.01f && rage < rageThreshold)
            {
                LeaperAI(player.Center, accelerationFactor: -0.07f, velMax: 4f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                fear = true;
            }

            // Attack with increasing speed
            if (rage >= rageThreshold)
            {
                LeaperAI(player.Center, accelerationFactor: 0.2f, velMax: 7f, maxJumpTilesX: 2, maxJumpTilesY: 1);
                fear = false;
            }

            NPC.despawnEncouraged = false;

            NPC.damage = NPC.defDamage;

            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.1f;

            timeSinceLastJump--;
        }

        // Adaption of FighterAI
        private void LeaperAI(Vector2 targetPosition, float accelerationFactor = 0.07f, float velMax = 1f, int maxJumpTilesX = 3, int maxJumpTilesY = 4, bool targetPlayers = true, bool jumpUpPlatforms = false, Action<bool, bool, Vector2, Vector2> onTileCollide = null, bool ignoreJumpTiles = false)
        {
            if (performingWallAnimation)
                return;

            if (targetPosition.X > NPC.Center.X)
                NPC.direction = 1;
            else
                NPC.direction = -1;

            if (NPC.velocity.X < -velMax || NPC.velocity.X > velMax)
            {
                if (NPC.velocity.Y == 0f)
                    NPC.velocity *= 0.8f;
            }
            else if (NPC.velocity.X < velMax && NPC.direction == 1)
            {
                NPC.velocity.X += accelerationFactor;

                if (NPC.velocity.X > velMax)
                    NPC.velocity.X = velMax;
            }
            else if (NPC.velocity.X > -velMax && NPC.direction == -1)
            {
                NPC.velocity.X -= accelerationFactor;

                if (NPC.velocity.X < -velMax)
                    NPC.velocity.X = -velMax;
            }

            NPC.WalkupHalfBricks();

            if (Utility.HitTileOnSide(NPC, 3))
            {
                if (NPC.velocity.X < 0f && NPC.direction == -1 || NPC.velocity.X > 0f && NPC.direction == 1)
                {
                    Vector2 newVec = Utility.AttemptJump(NPC.position, NPC.velocity, NPC.width, NPC.height, NPC.direction, targetPosition, NPC.directionY, maxJumpTilesX, maxJumpTilesY, velMax, jumpUpPlatforms, ignoreJumpTiles);
                    if (!NPC.noTileCollide)
                    {
                        newVec = Collision.TileCollision(NPC.position, newVec, NPC.width, NPC.height);
                        Vector4 slopeVec = Collision.SlopeCollision(NPC.position, newVec, NPC.width, NPC.height);
                        Vector2 slopeVel = new(slopeVec.Z, slopeVec.W);
                        if (onTileCollide != null && NPC.velocity != slopeVel)
                        {
                            onTileCollide(NPC.velocity.X != slopeVel.X, NPC.velocity.Y != slopeVel.Y, NPC.velocity, slopeVel);
                        }

                        NPC.position = new Vector2(slopeVec.X, slopeVec.Y);
                        NPC.velocity = slopeVel;
                    }

                    if (NPC.velocity != newVec)
                    {
                        NPC.velocity = newVec;
                        NPC.netUpdate = true;

                    }
                }
            }

            jumping = !Utility.HitTileOnSide(NPC, 3);

            if ((CheckRight() || CheckLeft()) && fear == false && !CheckUnder() && timeSinceLastJump < 1)
            {
                performingWallAnimation = true;
                NPC.frame.Y = 1056;
                NPC.velocity = Vector2.Zero;
            }
            else
            {
                performingWallAnimation = false;
            }
        }

        // Manages the leaper's rage
        private float GetRage(float lightlevel) 
        {
            if (lightlevel < lightValueFlee)
                return -0.03f; // Calms down when in darkness

            if (lightlevel >= lightValueFlee && lightlevel < lightValueRage)
                return 0.04f; // Becomes angry when in moderate light.

            if (lightlevel >= lightValueRage)
                return 0.09f; // Becomes enrages quickly while in bright light
                
            return 0f; // Fallback
        }

        private bool CheckRight()
        {
            float Dist1 = Utility.CastLength(NPC.Center + new Vector2(0, NPC.height / 2), new Vector2(1, 0), 40f, false);
            float Dist2 = Utility.CastLength(NPC.Center - new Vector2(0, NPC.height / 2), new Vector2(1, 0), 40f, false);
            return Dist2 < 18f && Dist1 < 18f;
        }

        private bool CheckLeft()
        {
            float Dist1 = Utility.CastLength(NPC.Center + new Vector2(0, NPC.height / 2), new Vector2(-1, 0), 40f, false);
            float Dist2 = Utility.CastLength(NPC.Center - new Vector2(0, NPC.height / 2), new Vector2(-1, 0), 40f, false);
            return Dist2 < 18f && Dist1 < 18f;
        }

        private bool CheckUnder()
        {
            float Dist1 = Utility.CastLength(NPC.Center, new Vector2(0, 1), 40f, false);
            return Dist1 < 30f;
        }

        // frames 0 - 9: idle 
        // frames 10 - 16: leap
        // frames 17 - 23: run 
        // frames 24 - 39: wall stick 
        private readonly Range idleFrames = 0..9;
        private readonly Range jumpingFrames = 10..16;
        private readonly Range runningFrame = 17..23;
        private readonly Range wallFrame = 24..39;

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 5;
            int frameIndex = NPC.frame.Y / frameHeight;
            NPC.frameCounter++;
            NPC.spriteDirection = fear == true ? -NPC.direction : NPC.direction; //Edtited this line to make the leaper face the right direction
            if (performingWallAnimation)
            {
                NPC.velocity = Vector2.Zero;
                // Update frame
                if (NPC.frameCounter > ticksPerFrame)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (frameIndex >= wallFrame.End.Value)
                {
                    performingWallAnimation = false;
                    NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 15f;
                    timeSinceLastJump = 60;
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
                    if (!idleFrames.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * idleFrames.Start.Value;

                    // Walking animation frame counter, accounting for walk speed
                    NPC.frameCounter += Math.Abs(NPC.velocity.X);

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= idleFrames.End.Value)
                        NPC.frame.Y = frameHeight * idleFrames.Start.Value;
                }
                else if (!jumping)
                {
                    // Reset walking 
                    if (!runningFrame.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * runningFrame.Start.Value;

                    // Walking animation frame counter, accounting for walk speed

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= runningFrame.End.Value)
                        NPC.frame.Y = frameHeight * runningFrame.Start.Value;
                }
                else if (jumping)
                {
                    // Reset walking 
                    if (!jumpingFrames.Contains(frameIndex))
                        NPC.frame.Y = frameHeight * jumpingFrames.Start.Value;

                    // Walking animation frame counter, accounting for walk speed

                    // Update frame
                    if (NPC.frameCounter > 5.0)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0.0;
                    }

                    if (frameIndex >= jumpingFrames.End.Value)
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