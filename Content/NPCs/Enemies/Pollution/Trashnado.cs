using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution
{
    public partial class Trashnado : ModNPC
    {
        private List<TrashData> trashEntities;
        private int trashOrbitTimer;
        private float trashRotation;
        private int smogTimer = 0;
        private int attackTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPC.ApplyBuffImmunity
            (
                BuffID.Bleeding,
                BuffID.BloodButcherer,
                BuffID.Poisoned,
                BuffID.Venom
            );

            NPCSets.MoonNPC[Type] = true;

            NPCSets.Material[Type] = NPCMaterial.Supernatural;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Wind);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Inorganic);
        }

        public override void SetDefaults()
        {
            NPC.width = 52;
            NPC.height = 68;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 400;

            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<PollutionBiome>() && Main.hardMode ? 1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        private bool spawned;

        public override void AI()
        {
            if (!spawned)
            {
                GetTrash();
                spawned = true;
            }

            if (smogTimer++ % 1 == 0)
            {
                Smoke smoke = Particle.Create<Smoke>((p) =>
                {
                    p.Position = NPC.Center + NPC.velocity * 4f + new Vector2(0, Main.rand.NextFloat(-35f, 35f));
                    p.Scale = new(Main.rand.NextFloat(0.1f, 0.4f));
                    p.Velocity = NPC.velocity * 0.25f;
                    p.Rotation = 0f;
                    p.Color = (new Color(162, 162, 162) * Main.rand.NextFloat(0.25f, 1f)).WithAlpha(215);
                    p.DrawLayer = ParticleDrawLayer.BeforeNPCs;
                    p.VanillaUpdate = false;
                    p.Opacity = NPC.Opacity * Main.rand.NextFloat(0.1f, 0.5f);
                    p.TimeToLive = 30;
                    p.FadeInNormalizedTime = 0.4f;
                    p.FadeOutNormalizedTime = 0.4f;
                    p.ScaleVelocity = new(0.08f);
                    p.WindFactor = Main.windSpeedCurrent > 0 ? 0.035f : 0.01f;
                });
            }

            NPC.TargetClosest(faceTarget: true);
            Player player = Main.player[NPC.target];

            Utility.AIFighter(NPC, ref NPC.ai, player.Center, accelerationFactor: 0.08f, velMax: 4f, maxJumpTilesX: 1, maxJumpTilesY: 4);

            if (NPC.velocity.Y > 0)
                NPC.velocity.Y *= 0.9f;

            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = -NPC.direction;

            trashOrbitTimer += 1;
            attackTimer++;
            if (Vector2.Distance(NPC.Center, player.Center) < 600f)
            {
                if (attackTimer > 23)
                {
                    Vector2 projVelocity = Utility.PolarVector(8f, -Main.rand.NextFloat(0, MathHelper.Pi));
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity + new Vector2(player.Center.X > NPC.Center.X ? 4 : -4, 0), ModContent.ProjectileType<TrashnadoProjectile>(), Utility.TrueDamage((int)(NPC.damage * 0.9f)), 1f, Main.myPlayer, ai1: NPC.target, ai2: 10f);
                    attackTimer = 0;
                }
            }
        }

        private void GetTrash()
        {
            trashEntities = new();
            for (int i = 0; i < Main.rand.Next(5, 8); i++)
            {
                var data = TrashData.RandomPool.Get();
                data.Randomize();
                trashEntities.Add(data);
            }
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
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            trashRotation += 0.01f;

            DrawTrash(spriteBatch, drawColor, true);

            SpriteEffects effect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int frameHeight = TextureAssets.Npc[Type].Height() / Main.npcFrameCount[Type];
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, new Vector2(TextureAssets.Npc[Type].Width() / 2f, frameHeight / 2f), NPC.scale, effect, 0f);

            DrawTrash(spriteBatch, drawColor, false);

            return NPC.IsABestiaryIconDummy;
        }

        private void DrawTrash(SpriteBatch spriteBatch, Color drawColor, bool isFront)
        {
            if (NPC.IsABestiaryIconDummy)
                return;

            if (trashEntities is null)
                GetTrash();

            for (int i = 0; i < trashEntities.Count; i++)
            {
                TrashData trash = trashEntities[i];
                float speedFactor = 7.5f - i;
                Color color = Utility.Colorize(trash.Color, drawColor);
                Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians((trashOrbitTimer + trash.Offset) * speedFactor)) * 45f, -(float)Math.Sin(MathHelper.ToRadians((trashOrbitTimer + trash.Offset) * speedFactor)) * 12f);

                if ((int)((trashOrbitTimer + trash.Offset) * speedFactor) % 360 < 180 == isFront)
                    spriteBatch.Draw(trash.Texture.Value, NPC.Center + orbit.RotatedBy(MathHelper.ToRadians(trash.Tilt)) - Main.screenPosition + new Vector2(0, -20 + (8 * i)), null, color, trashRotation, trash.Texture.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            }
        }
    }
}
