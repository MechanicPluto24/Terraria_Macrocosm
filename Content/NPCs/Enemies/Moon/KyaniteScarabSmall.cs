using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Tiles.Ambient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class KyaniteScarabSmall : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            NPCSets.MoonNPC[NPC.type] = true;
            NPCSets.DropsMoonstone[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 20;
            NPC.damage = 40;
            NPC.defense = 90;
            NPC.lifeMax = 300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60f;
            NPC.knockBackResist = 0.6f;
            NPC.aiStyle = -1;
            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
        }

        private int interestTimer = 0;
        private bool hasRock = false;

        public override void AI()
        {
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            Point luminiteCoords = Utility.GetClosestTile(NPC.Center, TileID.LunarOre, 100);
            Vector2 luminitePosition = luminiteCoords.ToWorldCoordinates();
            Point nestCoords = Utility.GetClosestTile(NPC.Center, ModContent.TileType<KyaniteNest>(), 200);
            Vector2 nestPosition = nestCoords.ToWorldCoordinates();

            if (Vector2.Distance(NPC.Center, target.Center) < 300f)
            {
                Utility.AIZombie(NPC, ref NPC.ai, fleeWhenDay: false, allowBoredom: true, ticksUntilBoredom: 20);
            }
            else
            {
                if (nestCoords != default && hasRock)
                {
                    Utility.AIFighter(NPC, ref NPC.ai, nestPosition);
                    if (Vector2.Distance(NPC.Center, nestPosition) < 17f){
                    NPC.active = false;
                    NPC.life = 0;
                    }
                }
                else
                {
                    if (luminiteCoords != default && !hasRock)
                    {
                        if (Vector2.Distance(NPC.Center, luminitePosition) > 17f && interestTimer < 600)
                        {
                            Utility.AIFighter(NPC, ref NPC.ai, luminitePosition);
                            interestTimer++;
                        }
                        else
                        {
                            NPC.velocity.X *= 0.01f;
                            if (interestTimer < 600)
                                interestTimer = 600;
                            if (Math.Abs(NPC.velocity.Y) < 0.01f)
                            {
                                interestTimer++;
                                Dust dust = Dust.NewDustDirect(luminitePosition, 16, 16, DustID.LunarOre);
                                dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f);
                                dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                                dust.noGravity = true;
                                if (interestTimer > 660)
                                {
                                    interestTimer = 0;
                                    hasRock = true;
                                }
                            }
                        }
                    }
                }
            }

            NPC.spriteDirection = NPC.direction;

        }

        public override void FindFrame(int frameHeight)
        {
            int ticksPerFrame = 8;
            NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + 0) * frameHeight;

            if (NPC.frameCounter++ >= (ticksPerFrame * (Main.npcFrameCount[Type] - 1)) - 1)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = 0 * frameHeight;
            }

            if (Math.Abs(NPC.velocity.Y) > 0.01f)
                NPC.frame.Y = (Main.npcFrameCount[Type] - 1) * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore3").Type);
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("KyaniteSmallGore2").Type);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D rockTexture = ModContent.Request<Texture2D>(Texture + "_Pebble").Value;
            SpriteEffects Effect = NPC.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = NPC.direction == -1 ? (NPC.Center + new Vector2(-4f, (float)(NPC.height / 2))) - Main.screenPosition : (NPC.Center + new Vector2(28f, (float)(NPC.height / 2))) - Main.screenPosition;
            Color colour = NPC.GetAlpha(drawColor);

            if (hasRock)
                spriteBatch.Draw(rockTexture, drawPos, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, Effect, 0f);
        }
    }
}
