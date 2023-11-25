using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class DianiteMeteorSmall : DianiteMeteor
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            float count = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) * 10f;

            if (count > 25f)
                count = 25f;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            for (int n = 2; n < count - 5; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.velocity * n * 0.15f;
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.OrangeRed * (0.8f - (float)n / count), Projectile.rotation + ((float)n / count), TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale * (1f - (float)n / count), SpriteEffects.None, 0f);
            }

            Projectile.GetTrail().Draw(TextureAssets.Projectile[Type].Size() / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f);
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.oldVelocity, (int)(Projectile.width), Projectile.height, DustID.Flare, velocity.X, velocity.Y, Scale: 2f);
                dust.noGravity = true;
            }

            //var smoke = Particle.CreateParticle<Smoke>(Projectile.Center + Projectile.oldVelocity, Vector2.Zero, scale: 0.6f);
            //smoke.Velocity *= 0.2f;
            //smoke.Velocity.X += Main.rand.Next(-10, 11) * 0.15f;
            //smoke.Velocity.Y += Main.rand.Next(-10, 11) * 0.15f;

            var explosion = Particle.CreateParticle<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity + Main.rand.NextVector2Circular(10f, 10f);
                p.DrawColor = (new Color(195, 115, 62)).WithOpacity(0.6f);
                p.Scale = 0.6f;
                p.NumberOfInnerReplicas = 4;
                p.ReplicaScalingFactor = 0.3f;
            });
        }
    }
}