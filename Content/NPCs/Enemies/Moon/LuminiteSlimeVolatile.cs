using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class LuminiteSlimeVolatile : LuminiteSlime
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax = 650;
            NPC.damage = 80;
            NPC.defense = 68;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return base.SpawnChance(spawnInfo);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SpawnDusts(4);

            if (NPC.life <= 0)
                SpawnDusts(45);
        }

        public override bool PreAI()
        {
            Utility.AISlime(NPC, ref NPC.ai, false, false, 140, 5, -8, 6, -12);

            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.25f;

            return true;
        }

        public override void AI()
        {
            base.AI();
        }

        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, default, ModContent.ProjectileType<LuminiteExplosion>(), (int)(NPC.damage * 0.35f), 3, Main.myPlayer);

            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(5f, 10f), ModContent.ProjectileType<LuminiteShard>(), (int)(NPC.damage * 0.25f), 1f, Main.myPlayer, ai1: -1, ai2: 1);
            }
            var explosion = Particle.CreateParticle<TintableExplosion>(p =>
                {
                    p.Position = NPC.Center;
                    p.DrawColor = (new Color(50, 200, 170)).WithOpacity(0.8f);
                    p.Scale = 1.7f;
                    p.NumberOfInnerReplicas = 8;
                    p.ReplicaScalingFactor = 0.4f;
                });
        }

        protected override void ProjectileAttack()
        {
            for (int i = 0; i < Main.rand.Next(3, 7); i++)
            {
                Vector2 projVelocity = Utility.PolarVector(2.6f, Main.rand.NextFloat(-MathHelper.Pi + MathHelper.PiOver4, -MathHelper.PiOver4));
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteStar>(), Utility.TrueDamage((int)(NPC.damage * 1.5f)), 1f, Main.myPlayer, ai1: NPC.target);
                proj.netUpdate = true;
            }

            SpawnDusts(5);
        }
    }
}