using Macrocosm.Common.Bases.NPCs;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.Food;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class Clavite : ComplexAINPC<Clavite.AIState>
    {
        public enum AIState
        {
            Fly,
            Dash
        }

        private static Asset<Texture2D> glowmask;
        private static Asset<Texture2D> glowX4;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 1;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults2()
        {
            NPC.width = 56;
            NPC.height = 56;
            NPC.lifeMax = 1300;
            NPC.damage = 45;
            NPC.defense = 30;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<AlienResidue>(), 4));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<Cheese>(), 50, 1, 20));
        }

        private Vector2? flyToOffset = null;

        [StateMethod(AIState.Fly)]
        private void Fly()
        {

            if (flyToOffset is null)
            {
                if ((NPC.target = GetRandomTargetInRange(1000)) == -1)
                {
                    return;
                }

                flyToOffset = Main.rand.NextVector2Unit() * 100f;
            }

            Player player = Main.player[NPC.target];


            NPC.Move(player.Center + flyToOffset.Value, Vector2.Zero, 5, 0.1f);
            NPC.velocity += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathF.Sin(Main.GameUpdateCount * 0.1f);
            NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;
            NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).ToRotation();


            if (NPC.Center.DistanceSQ(player.Center) < 200f * 200f)
            {
                flyToOffset = null;
                SetState(AIState.Dash);
            }

        }

        private readonly int dashWaitFrames = 40;
        private readonly int chompFrames = 6;

        [StateMethod(AIState.Dash)]
        private void Dash()
        {
            if (StateTime.Frames < dashWaitFrames)
            {
                NPC.velocity *= 0.98f;
                NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).ToRotation();

                if (StateTime.Frames == dashWaitFrames - 7)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                }
                return;
            }
            else if (StateTime.Frames == dashWaitFrames)
            {
                NPC.velocity = NPC.Center.DirectionTo(Main.player[NPC.target].Center) * 26f;
            }
            else if (StateTime.Frames == dashWaitFrames + chompFrames)
            {
                /*SoundEngine.PlaySound(SoundID.);*/
            }

            NPC.velocity *= 0.96f;
            NPC.rotation = NPC.velocity.ToRotation();
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            if (NPC.velocity.LengthSquared() < 6f)
            {
                SetState(AIState.Fly);
            }
            if (Utility.CastLength(NPC.Center, NPC.rotation.ToRotationVector2(), 82f, false) < 81f)
                NPC.velocity *= -0.5f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (State == AIState.Dash)
            {
                NPC.frame.Y = StateTime.Frames < dashWaitFrames + chompFrames ? 0 : frameHeight;
                return;
            }

            int frameSpeed = 15;

            NPC.frameCounter++;

            if (NPC.frameCounter >= frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? DustID.Stone : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (Main.dedServ)
                return;

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreJaw1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreJaw2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 1.5f, Mod.Find<ModGore>("ClaviteGoreEye1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreEye2").Type);

                for (int i = 0; i < 100; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? DustID.Stone : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            glowX4 ??= ModContent.Request<Texture2D>(Texture + "_GlowX4");

            if (NPC.IsABestiaryIconDummy)
                NPC.rotation = MathHelper.Pi;

            SpriteBatchState state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            Texture2D texture = TextureAssets.Npc[Type].Value;
            for (int i = 0; i < NPC.oldPos.Length * 0.5f; i++)
            {
                float factor = 1f - (float)i / (NPC.oldPos.Length + 1);
                spriteBatch.Draw(
                    texture,
                    NPC.oldPos[i] + NPC.Size * 0.5f - screenPos,
                    NPC.frame,
                    drawColor * factor * 0.5f,
                    NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                    NPC.frame.Size() * 0.5f,
                    NPC.scale,
                    NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            spriteBatch.Draw(
                texture,
                NPC.Center - screenPos,
                NPC.frame,
                drawColor,
                NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                NPC.frame.Size() * 0.5f,
                NPC.scale,
                NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );

            spriteBatch.Draw(
                glowmask.Value,
                NPC.Center - screenPos,
                NPC.frame,
                Color.White,
                NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                NPC.frame.Size() * 0.5f,
                NPC.scale,
                NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );


            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            Rectangle glowX4Source = new(NPC.frame.X * 4, NPC.frame.Y * 4, NPC.frame.Width * 4, NPC.frame.Height * 4);
            spriteBatch.Draw(
                glowX4.Value,
                NPC.Center - screenPos,
                glowX4Source,
                Color.White * 0.25f,
                NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                glowX4Source.Size() * 0.5f,
                NPC.scale / 4,
                NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float factor = 1f - (float)i / (NPC.oldPos.Length + 1);

                spriteBatch.Draw(
                    glowmask.Value,
                    NPC.oldPos[i] + NPC.Size * 0.5f - screenPos,
                    NPC.frame,
                    Color.White * factor * 0.66f,
                    NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                    NPC.frame.Size() * 0.5f,
                    NPC.scale,
                    NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0f
                );

                spriteBatch.Draw(
                    glowX4.Value,
                    NPC.oldPos[i] + NPC.Size * 0.5f - screenPos,
                    glowX4Source,
                    Color.White * 0.15f * factor,
                    NPC.direction == 1 ? NPC.rotation : NPC.rotation + MathHelper.Pi,
                    glowX4Source.Size() * 0.5f,
                    NPC.scale / 4,
                    NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }
    }
}
