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

        public bool Jumping = false;

        public void AILeaper(NPC npc, ref float[] ai, bool fleeWhenDay = true, bool allowBoredom = true, int openDoors = 1, float moveInterval = 0.07f, float velMax = 1f, int maxJumpTilesX = 3, int maxJumpTilesY = 4, int ticksUntilBoredom = 60, bool targetPlayers = true, int doorBeatCounterMax = 10, int doorCounterMax = 60, bool jumpUpPlatforms = false, Action<bool, bool, Vector2, Vector2> onTileCollide = null, bool ignoreJumpTiles = false)
        {
            bool xVelocityChanged = false;
            //This block of code checks for major X velocity/directional changes as well as periodically updates the npc.
            if (npc.velocity.Y == 0f && (npc.velocity.X > 0f && npc.direction < 0 || npc.velocity.X < 0f && npc.direction > 0))
            {
                xVelocityChanged = true;
            }
            if (npc.position.X == npc.oldPosition.X || ai[3] >= ticksUntilBoredom || xVelocityChanged)
            {
                ai[3] += 1f;
            }
            else
            if (Math.Abs(npc.velocity.X) > 0.9 && ai[3] > 0f) { ai[3] -= 1f; }
            if (ai[3] > ticksUntilBoredom * 10) { ai[3] = 0f; }
            if (npc.justHit) { ai[3] = 0f; }
            if (ai[3] == ticksUntilBoredom) { npc.netUpdate = true; }

            bool notBored = ai[3] < ticksUntilBoredom;
            //if npc does not flee when it's day, if is night, or npc is not on the surface and it hasn't updated projectile pass, update target.
            if (targetPlayers && (!fleeWhenDay || !Main.dayTime || npc.position.Y > Main.worldSurface * 16.0) && (fleeWhenDay && Main.dayTime ? notBored : !allowBoredom || notBored))
            {
                npc.TargetClosest();
            }
            else
            if (ai[2] <= 0f)//if 'bored'
            {
                if (fleeWhenDay && Main.dayTime && npc.position.Y / 16f < Main.worldSurface && npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        ai[0] += 1f;
                        if (ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            ai[0] = 0f;
                        }
                    }
                }
                else { ai[0] = 0f; }
                if (npc.direction == 0) { npc.direction = -1; }
            }
            //if velocity is less than -1 or greater than 1...
            if (npc.velocity.X < -velMax || npc.velocity.X > velMax)
            {
                //...and npc is not falling or jumping, slow down x velocity.
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= 0.8f;

                }
            }
            else
            if (npc.velocity.X < velMax && npc.direction == 1) //handles movement to the right. Clamps at velMaxX.
            {
                npc.velocity.X += moveInterval;
                if (npc.velocity.X > velMax) { npc.velocity.X = velMax; }
            }
            else
            if (npc.velocity.X > -velMax && npc.direction == -1) //handles movement to the left. Clamps at -velMaxX.
            {
                npc.velocity.X -= moveInterval;
                if (npc.velocity.X < -velMax) { npc.velocity.X = -velMax; }
            }
            Utility.WalkupHalfBricks(npc);
            //if allowed to open doors and is currently doing so, reduce npc velocity on the X axis to 0. (so it stops moving)
            if (openDoors != -1 && Utility.AttemptOpenDoor(npc, ref ai[1], ref ai[2], ref ai[3], ticksUntilBoredom, doorBeatCounterMax, doorCounterMax, openDoors))
            {
                npc.velocity.X = 0;
            }
            else //if no door to open, reset ai.
            if (openDoors != -1) { ai[1] = 0f; ai[2] = 0f; }
            //if there's a solid floor under us...
            if (Utility.HitTileOnSide(npc, 3))
            {
                //if the npc's velocity is going in the same direction as the npc's direction...
                if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                {
                    //...attempt to jump if needed.
                    Vector2 newVec = Utility.AttemptJump(npc.position, npc.velocity, npc.width, npc.height, npc.direction, npc.directionY, maxJumpTilesX, maxJumpTilesY, velMax, jumpUpPlatforms, jumpUpPlatforms && notBored ? Main.player[npc.target] : null, ignoreJumpTiles);
                    if (npc.noTileCollide)
                    {
                        Jumping = true;
                    }
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
                    if (npc.velocity != newVec) { npc.velocity = newVec; npc.netUpdate = true; }
                }
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
            Rage += RageManager(Lighting.GetColor(NPC.Center.ToTileCoordinates()).GetBrightness()); //manage rage.
            //put caps on rage.
            if (Rage > RageThreshold)
                Rage = RageThreshold;
            if (Rage < 0f)
                Rage = 0f;


            if (Rage <= 0.01f)
            {//Pure darkness, stalk the player slowly.
                AILeaper(NPC, ref NPC.ai, false, true, velMax: 2, maxJumpTilesX: 15, maxJumpTilesY: 10, moveInterval: 0.06f);
                Fear = false;
            }
            if (Rage > 0.01f && Rage < RageThreshold)
            {//Flee.
                AILeaper(NPC, ref NPC.ai, false, true, velMax: 4, maxJumpTilesX: 15, maxJumpTilesY: 10, moveInterval: -0.075f);
                Fear = true;
            }
            if (Rage >= RageThreshold)
            {//Attack with increasing speed.
                AILeaper(NPC, ref NPC.ai, false, true, velMax: 7, maxJumpTilesX: 18, maxJumpTilesY: 12, moveInterval: 0.08f + (Rage / 100f));
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
            NPC.spriteDirection = Fear == true ? -NPC.direction : NPC.direction; //Edtited this line to make the leaper face the right direction

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
            else if (!Jumping)
            {
                NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + runFrameInitial) * frameHeight;

                if (NPC.frameCounter >= ticksPerFrame * runFrameCount)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = runFrameInitial * frameHeight;
                }
            }
            else if (Jumping)
            {
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
