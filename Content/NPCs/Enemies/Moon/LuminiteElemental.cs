using Macrocosm.Common.DataStructures;
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
using System.IO;
using Terraria;
using Terraria.Audio;
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
            Panicking
        }

        public ActionState AI_State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public bool Healing
        {
            get => NPC.ai[1] > 0f;
            set => NPC.ai[0] = value ? 1f : 0f;
        }

        public ref float AI_Timer => ref NPC.ai[2];
        public ref float AI_Speed => ref NPC.ai[3];

        public Vector2 TargetPosition { get; set; } = default;
        public int HealTimer { get; set; } = 0;

        public bool InTiles => NPC.active && Main.tile[NPC.Center.ToTileCoordinates()].HasTile;

        private int pebbleOrbitTimer;
        private float pebbleRotation;

        private float starScale;
        private Color starColor;

        private int attackPeriod;
        private int panicAttackPeriod;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 38;
            NPC.damage = 75;
            NPC.defense = 80;
            NPC.lifeMax = 580;
            NPC.HitSound = SoundID.Dig;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 60f;
            NPC.knockBackResist = 0.2f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            attackPeriod = 180;
            panicAttackPeriod = 30;

            SpawnModBiomes = [ModContent.GetInstance<MoonUndergroundBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.SpawnTileY > Main.rockLayer && spawnInfo.SpawnTileType == ModContent.TileType<Protolith>()) ? 0.025f : 0f;
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

        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            if (Main.rand.NextBool(25))
                SpawnDusts();
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            if (NPC.HasPlayerTarget && clearLineOfSight)
                AI_State = ActionState.Attacking;
            else
                AI_State = ActionState.Idle;

            if (NPC.life < NPC.lifeMax / 3)
                AI_State = ActionState.Panicking;

            switch (AI_State)
            {
                case ActionState.Idle:
                    Idle();
                    break;
                case ActionState.Attacking:
                    Attack();
                    break;
                case ActionState.Panicking:
                    Panic();
                    break;
            }

            AI_Timer++;

            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = NPC.direction;

            Lighting.AddLight(NPC.Center - NPC.velocity, new Vector3(0.36f, 0.89f, 0.64f) * (InTiles ? 0.2f : 1f));

            pebbleOrbitTimer += 2;
            if (pebbleOrbitTimer >= 180)
                pebbleOrbitTimer = 0;
        }

        public void Idle()
        {
            if (AI_Timer % 100 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 offset = new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200));
                TargetPosition = NPC.Center + offset;
                AI_Timer = 0;
                NPC.netUpdate = true;
            }

            Vector2 direction = (TargetPosition - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = ((NPC.velocity + (direction * 0.1f)).SafeNormalize(Vector2.UnitX)) * 0.6f;
            AI_Speed = 1f;
        }

        public void Attack()
        {
            Player target = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

            if (!clearLineOfSight)
                AI_Timer--;

            if (AI_Timer >= attackPeriod)
            {
                if (clearLineOfSight && target.active && !target.dead && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.rand.Next(3, 6); i++)
                    {
                        Vector2 projVelocity = Utility.PolarVector(8f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                        Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 10f);
                        proj.netUpdate = true;
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

        public void Panic()
        {
            Player target = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

            if (!clearLineOfSight)
                AI_Timer--;

            if (AI_Timer >= panicAttackPeriod)
            {
                if (clearLineOfSight && target.active && !target.dead && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 projVelocity = Utility.PolarVector(12f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteShard>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 10f);
                    proj.netUpdate = true;
                }

                AI_Timer = 0;
            }

            Point luminiteCoords = Utility.GetClosestTile(NPC.Center, TileID.LunarOre, 100);
            if (luminiteCoords != default)
            {
                Vector2 luminitePosition = luminiteCoords.ToWorldCoordinates();

                if (Healing)
                    NPC.velocity *= 0.92f;
                else
                    NPC.velocity = (luminitePosition - NPC.Center).SafeNormalize(Vector2.UnitX) * 5f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Vector2.DistanceSquared(NPC.Center, luminitePosition) < 30 * 30 && NPC.life < NPC.lifeMax)
                    {
                        Healing = true;

                        if (HealTimer++ % 10 == 0)
                        {
                            NPC.life += 10;
                            NPC.HealEffect(10, broadcast: true);
                            NPC.netUpdate = true;
                        }
                    }
                    else 
                    {
                        Healing = false;
                        NPC.netUpdate = true;
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


        SpriteBatchState state;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            pebbleTexture ??= ModContent.Request<Texture2D>(Texture + "_Pebble");
            pebbleRotation += 0.001f;
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 45f, -(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f);

            if ((-(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f) < 0f)
                DrawPebbles(spriteBatch, drawColor, orbit);

            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, TextureAssets.Npc[Type].Size() / 2, NPC.scale, NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            DrawStar(spriteBatch);

            if ((-(float)Math.Sin(MathHelper.ToRadians(pebbleOrbitTimer * 2)) * 12f) >= 0f)
                DrawPebbles(spriteBatch, drawColor, orbit);

            return NPC.IsABestiaryIconDummy;
        }

        private void DrawPebbles(SpriteBatch spriteBatch, Color drawColor, Vector2 orbit)
        {
            int trailLength = 50;
            float opacityFactor = 0.1f;
            for (int i = 0; i < trailLength; i++)
            {
                float lerpFactor = i / (float)trailLength;
                Vector2 previousOrbit = new
                (
                    (float)Math.Cos(MathHelper.ToRadians((pebbleOrbitTimer - i) * 2)) * 45f, 
                    -(float)Math.Sin(MathHelper.ToRadians((pebbleOrbitTimer - i) * 2)) * 12f
                );

                Color trailColor = drawColor * (1f - lerpFactor) * opacityFactor;

                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + previousOrbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, trailColor, pebbleRotation, pebbleTexture.Size() / 2, NPC.scale * (1f - lerpFactor), SpriteEffects.None, 0f);
                spriteBatch.Draw(pebbleTexture.Value, NPC.Center + previousOrbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, trailColor, pebbleRotation, pebbleTexture.Size() / 2, NPC.scale * (1f - lerpFactor), SpriteEffects.FlipHorizontally, 0f);
            }

            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(-30)) - Main.screenPosition, null, drawColor, pebbleRotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(pebbleTexture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(46)) - Main.screenPosition, null, drawColor, pebbleRotation, pebbleTexture.Size() / 2, NPC.scale, SpriteEffects.FlipHorizontally, 0f);
        }

        private void DrawStar(SpriteBatch spriteBatch)
        {
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            starTexture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Star1");

            Color targetColor = new Color(100, 243, 172).WithOpacity(Main.rand.NextFloat(0.8f, 1f)) * Main.rand.NextFloat(1f, 1.2f);
            float targetScale = NPC.scale * 0.02f * Main.rand.NextFloat(0.8f, 1f);

            if (AI_State == ActionState.Attacking)
            {
                targetColor *= 1f + 0.5f * (AI_Timer / attackPeriod);
                targetScale += 0.1f * (AI_Timer / attackPeriod);
            }

            if (AI_State == ActionState.Panicking)
            {
                targetColor *= 1f + 0.5f * (AI_Timer / panicAttackPeriod);
                targetScale += 0.1f * (AI_Timer / panicAttackPeriod);
            }

            if (InTiles)
            {
                targetColor.A = 50;
                targetScale *= 0.5f;
            }

            starColor = Color.Lerp(targetColor, starColor, 1f - 0.075f);
            starScale = MathHelper.Lerp(targetScale, starScale, 1f - 0.075f);

            Vector2 position = ((NPC.Center + new Vector2(3.5f * NPC.spriteDirection, -1.5f)) + NPC.velocity.SafeNormalize(Vector2.UnitX) * 1.1f) - Main.screenPosition;
            spriteBatch.Draw(starTexture.Value, position, null, starColor, NPC.rotation, starTexture.Size() / 2, starScale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(TargetPosition);
            writer.Write(HealTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TargetPosition = reader.ReadVector2();
            HealTimer = reader.ReadInt32();
        }
    }
}
