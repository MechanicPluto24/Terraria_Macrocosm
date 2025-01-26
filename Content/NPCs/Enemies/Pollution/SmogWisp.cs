using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Pollution
{
    public class SmogWisp : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;

            NPC.ApplyImmunity
            (
                BuffID.Bleeding,
                BuffID.BloodButcherer,
                BuffID.Poisoned,
                BuffID.Venom
            );
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 20;
            NPC.damage = 12;
            NPC.defense = 10;
            NPC.lifeMax = 30;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            SpawnModBiomes = [ModContent.GetInstance<PollutionBiome>().Type];
            NPC.Opacity = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.Player.InModBiome<PollutionBiome>() && ((spawnInfo.SpawnTileY < spawnInfo.Player.Center.Y + 40))) ? 1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void FindFrame(int frameHeight)
        {
            int frameSpeed = 6;
            NPC.frameCounter++;

            if (NPC.frameCounter >= frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        int smogTimer = 0;
        public override void AI()
        {
            if (++smogTimer % 5 == 0)
            {
                Smoke smoke = Particle.Create<Smoke>((p) =>
                {
                    p.Position = NPC.Center + new Vector2(0, 12) * NPC.direction;
                    p.Velocity = -NPC.velocity.RotatedByRandom(MathHelper.PiOver2) * 0.1f;
                    p.Acceleration = new Vector2(0f, 0f);
                    p.Scale = new(0.4f);
                    p.Rotation = 0f;
                    p.Color = (new Color(80, 80, 80) * Main.rand.NextFloat(0.75f, 1f)).WithAlpha(215);
                    p.Opacity = NPC.Opacity;
                    p.ScaleVelocity = new(-0.0085f);
                });
            }

            Utility.AIFlier(NPC, ref NPC.ai, sporadic: true, moveIntervalX: 0.2f, moveIntervalY: 0.2f, maxSpeedX: 2f, maxSpeedY: 2f, canBeBored: false);

            if (NPC.Opacity < 0.01f)
                NPC.dontTakeDamage = true;
            else
                NPC.dontTakeDamage = false;

            Player player = Main.player[NPC.target];
            if (player is not null)
            {
                NPC.rotation = NPC.Center.DirectionTo(Main.player[NPC.target].Center).RotatedBy(MathHelper.Pi).ToRotation();
                if (Vector2.Distance(player.Center, NPC.Center) > 1000f)
                    NPC.Opacity -= 0.01f;
                else if (Vector2.Distance(player.Center, NPC.Center) > 500f)
                    NPC.Opacity += 0.001f;
                else
                    NPC.Opacity += 0.01f;
            }

            if (NPC.Opacity > 0.7f)
                NPC.Opacity = 0.7f;

            if (NPC.Opacity < 0f)
                NPC.Opacity = 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CoalDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 0.7f;

            return true;
        }
    }
}