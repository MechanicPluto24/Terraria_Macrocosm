using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.NPCs.Enemies.Moon.MoonLich
{
    public class MoonLich : ModNPC
    {
        private static Asset<Texture2D> handTexture;

        private float offsetY = 0f;
        private int timer;
        private bool summoned = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 46;
            NPC.lifeMax = 14000;
            NPC.damage = 60;
            NPC.defense = 75;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            NPC.value = 100f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && spawnInfo.SpawnTileY > Main.rockLayer ? .002f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>(), 1, 3, 5));
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            Lighting.AddLight(NPC.Center, (new Vector3(0.4f, 1f, 1f)));
            offsetY = (float)(Math.Sin(timer / 10) * 7);
            timer++;
            if (timer % 5 == 0)
                Particle.CreateParticle<TintableFire>(p =>
                {
                    p.Position = NPC.position;
                    p.Velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    p.DrawColor = new Color(100, 255, 255, 0);
                    p.Scale = 0.1f;
                });

            if (clearLineOfSight && player.active && !player.dead)
            {
                NPC.Move(player.Center, Vector2.Zero, 5, 0.1f);
                NPC.velocity += NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * MathF.Sin(Main.GameUpdateCount * 0.05f);
            }

            if (timer % 80 == 79)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 projVelocity = Main.player[NPC.target].Center - NPC.Center;
                    projVelocity = projVelocity.SafeNormalize(Vector2.UnitX);
                    projVelocity = (projVelocity + Main.player[NPC.target].velocity * 0.1f).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity * 14f, ModContent.ProjectileType<LichBolt>(), Utility.TrueDamage((int)(NPC.damage * 1.15f)), 1f, Main.myPlayer, ai1: NPC.target);
                }
            }

            if (clearLineOfSight && player.active && !player.dead && summoned == false)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 projVelocity = Utility.PolarVector(1.5f, Main.rand.NextFloat(0, MathHelper.Pi * 2));
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<MoonLichNPCSummon>(), Utility.TrueDamage((int)(0)), 1f, Main.myPlayer, NPC.target);
                    proj.netUpdate = true;
                }
                summoned = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = (NPC.Center + new Vector2(35f, 50f + offsetY)) - Main.screenPosition;
            Color colour = NPC.GetAlpha(drawColor);

            //Right
            handTexture ??= ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/MoonLich/MoonLichHand");

            spriteBatch.Draw(handTexture.Value, drawPos, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0f);

            Vector2 drawPos2 = (NPC.Center + new Vector2(-35f, 50f + offsetY)) - Main.screenPosition;
            spriteBatch.Draw(handTexture.Value, drawPos2, null, colour, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0f);

            return true;
        }



        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = -NPC.direction;
            int frameSpeed = 20;

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
            for (int i = 0; i < 8; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (Main.dedServ)
                return;

            if (NPC.life <= 0)
            {

                for (int i = 0; i < 50; i++)
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
}
