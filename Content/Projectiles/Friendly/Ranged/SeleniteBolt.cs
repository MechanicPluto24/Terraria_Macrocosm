using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Global.Projectiles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class SeleniteBolt : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public float Strenght
        {
            get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
        }

        float trailMultiplier = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 3;

            Projectile.CritChance = 16;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (trailMultiplier < 1f)
                trailMultiplier += 0.006f;

            Lighting.AddLight(Projectile.Center, new Color(177, 230, 204).ToVector3() * 0.6f);
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(64, 64);
            Projectile.knockBack = 6f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 3;
            Projectile.velocity *= 0f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);

            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<SeleniteBrightDust>(), Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            for (int i = 0; i < 30; i++)
            {
                Particle.CreateParticle<SeleniteSpark>(Projectile.Center + Projectile.oldVelocity, Main.rand.NextVector2Circular(9, 2), 5f, 0f);
                Particle.CreateParticle<SeleniteSpark>(Projectile.Center + Projectile.oldVelocity, Main.rand.NextVector2Circular(2, 9), 5f, 0f);
            }

            float count = Projectile.oldVelocity.LengthSquared() * trailMultiplier;
            for (int i = 1; i < count; i++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * i * 0.4f;
                for (int j = 0; j < 2; j++)
                {
                    Dust dust = Dust.NewDustDirect(trailPosition, 1, 1, ModContent.DustType<SeleniteBrightDust>(), 0, 0, Scale: Main.rand.NextFloat(1f, 2f) * (1f - i / count));
                    dust.noGravity = true;
                }
            }
        }

        SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            float count = Projectile.velocity.LengthSquared() * trailMultiplier;

            for (int n = 1; n < count; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.4f;
                Color color = new Color(177, 230, 204) * (0.8f - (float)n / count);
                spriteBatch.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * 0.65f, Projectile.rotation, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }

    }
}
