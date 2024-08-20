using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    //TODO Clean up this mess, make trails and add tile targeting.
    public class LuminiteElemental : ModNPC
    {
        private static Asset<Texture2D> pebbleTexture;
        private static Asset<Texture2D> starTexture;

        public enum ActionState
        {
            Idle,
            Attacking,
            Fleeing
        }

        public ActionState AI_State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_Speed => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 38;
            NPC.height = 38;
            NPC.damage = 75;
            NPC.defense = 80;
            NPC.lifeMax = 580;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.Dig;
            NPC.value = 60f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            SpawnModBiomes = [ModContent.GetInstance<UndergroundMoonBiome>().Type];
        }


        private Vector2 targetPosition = default;
        private int animateTimer;

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.07f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ItemID.LunarOre, chanceDenominator: 2, minimumDropped: 2, maximumDropped: 12));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SpawnDusts(3);

            if (NPC.life <= 0)
                SpawnDusts(15);
        }

        public override bool PreAI()
        {
            NPC.TargetClosest(true);

            if (Main.rand.NextBool(25))
                SpawnDusts();

            if (!NPC.HasPlayerTarget || Main.player[NPC.target].Distance(NPC.Center) > 800f)
                AI_State = ActionState.Idle;

            if (NPC.HasPlayerTarget && Main.player[NPC.target].Distance(NPC.Center) <= 800f)
                AI_State = ActionState.Attacking;

            if (NPC.life < NPC.lifeMax / 3)
                AI_State = ActionState.Fleeing;

            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.36f, 0.89f, 0.64f);

            switch (AI_State)
            {
                case ActionState.Idle:
                    Idle();
                    break;
                case ActionState.Attacking:
                    Attack();
                    break;
                case ActionState.Fleeing:
                    Flee();
                    break;
            }

            AI_Timer++;

            animateTimer += 2;
            if (animateTimer >= 180)
                animateTimer = 0;
        }

        public void Idle()
        {
            if (AI_Timer % 100 == 0)
            {
                Vector2 offset = new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200));
                targetPosition = NPC.Center + offset;
                AI_Timer = 0;
            }

            Vector2 direction = (targetPosition - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = ((NPC.velocity + (direction * 0.1f)).SafeNormalize(Vector2.UnitX)) * 0.6f;
            AI_Speed = 1f;
        }

        public void Attack()
        {
            if (AI_Timer % 180 == 0)
            {
                Player target = Main.player[NPC.target];
                bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                if (clearLineOfSight && target.active && !target.dead)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < Main.rand.Next(3, 7); i++)
                        {
                            Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, NPC.target);
                            proj.netUpdate = true;
                        }
                    }
                }
                AI_Timer = 0;
            }

            Player player = Main.player[NPC.target];

            AI_Speed += 0.03f;
            if (AI_Speed > 6f)
                AI_Speed = 6f;

            Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
        }

        public void Flee()
        {
            if (AI_Timer % 90 == 0)
            {
                Player target = Main.player[NPC.target];
                bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                if (clearLineOfSight && target.active && !target.dead)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < Main.rand.Next(1, 3); i++)
                        {
                            Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, NPC.target);
                            proj.netUpdate = true;
                        }
                    }
                }

                AI_Timer = 0;
            }

            Point luminiteCoords = Utility.GetClosestTile(NPC.Center, TileID.LunarOre, 100);
            if (luminiteCoords != default)
            {
                Vector2 luminitePosition = luminiteCoords.ToWorldCoordinates();
                NPC.velocity = (luminitePosition - NPC.Center).SafeNormalize(Vector2.UnitX) * 5f;

                if(Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Vector2.DistanceSquared(NPC.Center, luminitePosition) < 30 * 30 && NPC.life < NPC.lifeMax)
                    {
                        NPC.life++;
                        NPC.netUpdate = true;

                        if (animateTimer % (10 * 2) == 0)
                            NPC.HealEffect(10, broadcast: true);
                    }
                }
            }
            else
            {
                Player player = Main.player[NPC.target];
                AI_Speed += 0.03f;

                if (AI_Speed > 9f)
                    AI_Speed = 9f;

                Vector2 direction = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                NPC.velocity = ((NPC.velocity + (direction * 0.8f)).SafeNormalize(Vector2.UnitX)) * AI_Speed;
            }

            SpawnDusts(1);
        }

        public void SpawnDusts(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            pebbleTexture ??= ModContent.Request<Texture2D>(Texture + "_Pebble");

            //first rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f) < 0f)
            {
                Vector2 orbit = new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f);
                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, Color.White, NPC.rotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }

            //second rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f) < 0f)
            {
                Vector2 orbit = new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f);
                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, Color.White, NPC.rotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, TextureAssets.Npc[Type].Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            starTexture ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Star6");
            spriteBatch.Draw(starTexture.Value, ((NPC.Center + new Vector2(5f, -1f)) + NPC.velocity.SafeNormalize(Vector2.UnitX) * 1.1f) - Main.screenPosition, null, new Color(0.36f, 0.89f, 0.64f, 0), NPC.rotation, starTexture.Size() / 2, NPC.scale * 0.04f, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //first rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f) >= 0f)
            {
                Vector2 orbit = new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f);
                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, Color.White, NPC.rotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }
            //second rock 
            if ((-(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f) >= 0f)
            {
                Vector2 orbit = new Vector2((float)Math.Cos(MathHelper.ToRadians(animateTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(animateTimer * 2)) * 12f);
                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, Color.White, NPC.rotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }
        }
    }
}
