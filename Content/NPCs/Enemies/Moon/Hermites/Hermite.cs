using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Hermites
{
    public class Hermite : ModNPC
    {
        private float alert = 0f;
        private bool hasEmerged = true;
        private bool isCrawling = false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 14;

            NPCSets.MoonNPC[Type] = true;

            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Inorganic);
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 26;
            NPC.damage = 55;
            NPC.defense = 100;
            NPC.lifeMax = 1300;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_SpawnNPC)
                hasEmerged = false;
        }
        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];
            if (!isCrawling)
            {
                NPC.noGravity = false;

                if (Vector2.Distance(NPC.Center, player.Center) < 200f)
                    alert += 0.01f;

                if (alert > 0.6f && hasEmerged)
                {
                    Utility.AIZombie(NPC, ref NPC.ai, fleeWhenDay: false, allowBoredom: false, velMax: 5, maxJumpTilesX: 1, maxJumpTilesY: 1, moveInterval: 0.15f);
                    NPC.dontTakeDamage = false;
                    NPC.spriteDirection = NPC.Center.X < player.Center.X ? -1 : 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.velocity.Y == 0f)
                    {
                        int num = (int)NPC.Center.X / 16;
                        int num2 = (int)NPC.Center.Y / 16;
                        bool flag = false;

                        for (int i = num - 1; i <= num + 1; i++)
                            for (int j = num2 - 1; j <= num2 + 1; j++)
                                if (Main.tile[i, j] != null && Main.tile[i, j].WallType > 0)
                                    flag = true;

                        if (flag)
                            isCrawling = true;
                    }
                }
                else
                {
                    NPC.dontTakeDamage = true;
                }
            }
            else
            {
                float MaxSpeed = 2f;
                float Acceleration = 0.08f;
                NPC.noGravity = true;

                if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
                    NPC.TargetClosest();

                Vector2 vector = Main.player[NPC.target].Center - NPC.Center;
                float distance = vector.Length();

                if (distance == 0f)
                {
                    vector.X = NPC.velocity.X;
                    vector.Y = NPC.velocity.Y;
                }
                else
                {
                    distance = MaxSpeed / distance;
                    vector.X *= distance;
                    vector.Y *= distance;
                }

                if (Main.player[NPC.target].dead)
                {
                    vector.X = NPC.direction * MaxSpeed / 2f;
                    vector.Y = -MaxSpeed / 2f;
                }

                if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position,
                    Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.ai[0] += 1f;
                    if (NPC.ai[0] > 0f)
                        NPC.velocity.Y += 0.023f;
                    else
                        NPC.velocity.Y -= 0.023f;

                    if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                        NPC.velocity.X += 0.023f;
                    else
                        NPC.velocity.X -= 0.023f;
                    if (NPC.ai[0] > 200f)
                        NPC.ai[0] = -200f;

                    NPC.velocity.X += vector.X * 0.007f;
                    NPC.velocity.Y += vector.Y * 0.007f;

                    NPC.rotation = vector.ToRotation() + 1.57f;

                    if (NPC.velocity.X > 1.5f || NPC.velocity.X < -1.5f)
                        NPC.velocity.X *= 0.9f;
                    if (NPC.velocity.Y > 1.5f || NPC.velocity.Y < -1.5f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -3f, 3f);
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -3f, 3f);
                }
                else
                {
                    if (NPC.velocity.X < vector.X)
                    {
                        NPC.velocity.X += Acceleration;
                        if (NPC.velocity.X < 0f && vector.X > 0f)
                            NPC.velocity.X += Acceleration;
                    }
                    else if (NPC.velocity.X > vector.X)
                    {
                        NPC.velocity.X -= Acceleration;
                        if (NPC.velocity.X > 0f && vector.X < 0f)
                            NPC.velocity.X -= Acceleration;
                    }
                    if (NPC.velocity.Y < vector.Y)
                    {
                        NPC.velocity.Y += Acceleration;
                        if (NPC.velocity.Y < 0f && vector.Y > 0f)
                            NPC.velocity.Y += Acceleration;

                    }
                    else if (NPC.velocity.Y > vector.Y)
                    {
                        NPC.velocity.Y -= Acceleration;
                        if (NPC.velocity.Y > 0f && vector.Y < 0f)
                            NPC.velocity.Y -= Acceleration;
                    }

                    NPC.rotation = vector.ToRotation();
                }

                float num2 = 0.5f;

                if (NPC.collideX)
                {
                    NPC.netUpdate = true;
                    NPC.velocity.X = NPC.oldVelocity.X * -num2;

                    if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                        NPC.velocity.X = 2f;

                    if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                        NPC.velocity.X = -2f;
                }

                if (NPC.collideY)
                {
                    NPC.netUpdate = true;
                    NPC.velocity.Y = NPC.oldVelocity.Y * -num2;

                    if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1.5f)
                        NPC.velocity.Y = 2f;

                    if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1.5f)
                        NPC.velocity.Y = -2f;
                }

                if ((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f &&
                    NPC.oldVelocity.Y < 0f || NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f) && !NPC.justHit)
                    NPC.netUpdate = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int npcX = (int)NPC.Center.X / 16;
                    int npcY = (int)NPC.Center.Y / 16;
                    bool flag = false;

                    for (int i = npcX - 1; i <= npcX + 1; i++)
                        for (int j = npcY - 1; j <= npcY + 1; j++)
                        {
                            if (Main.tile[i, j] == null)
                                break;

                            if (Main.tile[i, j].WallType > 0)
                                flag = true;
                        }

                    if (!flag)
                        isCrawling = false;
                }
            }

        }
        int frameCounter = 0;
        int frameTimer;
        Rectangle altFrame;
        int altFrameCounter = 0;
        public override void FindFrame(int frameHeight)
        {
            if (alert <= 0.6f)
                frameCounter = 0;
            else if (alert > 0.6f && !hasEmerged)
            {
                frameTimer++;
                if (frameTimer > 10)
                {
                    frameCounter++;
                    frameTimer = 0;
                }
                if (frameCounter > 2)
                    hasEmerged = true;
            }
            else
            {
                frameTimer++;
                if (frameTimer > 7)
                {
                    frameCounter++;
                    frameTimer = 0;
                    altFrameCounter++;
                }
                if (frameCounter > 13)
                    frameCounter = 3;
                if (altFrameCounter > 3)
                    altFrameCounter = 0;
            }
            Asset<Texture2D> frametexture = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "Wall");
            NPC.frame.Y = frameCounter * frameHeight;
            altFrame = new Rectangle(0, 0, frametexture.Width(), frametexture.Height() / 4);

            altFrame.Y = frametexture.Height() / 4 * altFrameCounter;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "Wall").Value;

            if (!isCrawling)
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, drawColor, 0f, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (isCrawling)
                spriteBatch.Draw(texture, NPC.Center - screenPos, altFrame, drawColor, NPC.rotation, altFrame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Blocks.Terrain.Regolith>() && Main.dayTime ? 0.08f : 0f;

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Regolith>(), 2));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }
    }
}