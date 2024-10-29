using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class DeliriumShell : ModProjectile
    {
        private static Asset<Texture2D> aura;
        public override void Load()
        {
            aura = ModContent.Request<Texture2D>(Texture + "Aura");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Explosive[Type] = true;
            ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            //Projectile.scale = 1.2f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
        }

        bool spawned = false;
        float auraAlpha = 0f;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            Lighting.AddLight(Projectile.Center, new Color(101, 242, 139).ToVector3());

            if (auraAlpha < 1f)
                auraAlpha += 0.06f;

            if (!spawned && auraAlpha > 0.1f)
            {
                // spawn some dusts as "muzzle flash"
                for (int i = 0; i < 55; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<GreenBrightDust>(), Scale: 1.8f);
                    dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1.4f, 12f)).RotatedByRandom(MathHelper.ToRadians(18)) + Main.player[Projectile.owner].velocity;
                    dust.noLight = false;
                    dust.noGravity = false;
                }
                spawned = true;
            }

            // spawn dust trail 
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GreenBrightDust>(), Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, Scale: 1.2f);
                dust.noLight = false;
                dust.noGravity = false;
            }

            if (Projectile.alpha > 0)
                Projectile.alpha -= 10;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(100, 100);
            Projectile.knockBack = 4f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;

            Projectile.velocity *= 0f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            var explosion = Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity;
                p.Color = new Color(96, 237, 134) * 0.6f;
                p.Scale = new(1.2f);
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 0.5f;
            });

            //spawn dust explosion on kill
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<GreenBrightDust>(), Scale: 2.4f);
                dust.velocity = (Vector2.UnitX * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
                dust.noLight = false;
                dust.noGravity = false;
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 3)
                return false;

            var spriteBatch = Main.spriteBatch;
            state.SaveState(Main.spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(aura.Value, Projectile.Center - new Vector2(0, 38).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.3f * auraAlpha), Projectile.rotation - MathHelper.Pi, aura.Size() / 2, 0.8f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(aura.Value, Projectile.Center - new Vector2(0, 58).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.1f * auraAlpha), Projectile.rotation - MathHelper.Pi, aura.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(aura.Value, Projectile.Center - new Vector2(0, 18).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.6f), Projectile.rotation - MathHelper.Pi, aura.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(state);
            return true;
        }
    }
}
