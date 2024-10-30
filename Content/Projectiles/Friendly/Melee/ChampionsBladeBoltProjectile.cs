using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    internal class ChampionsBladeBoltProjectile : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.aiStyle = -1;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 80;
            Projectile.extraUpdates = 2;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;

            Projectile.alpha = 255;


            ProjectileID.Sets.TrailCacheLength[Type] = Main.rand.Next(15, 25);
            visualScale = Main.rand.NextFloat(0.8f, 1.5f);
        }

        private ref float SkewMultiplier => ref Projectile.ai[0];
        private float visualScale;
        public override void OnSpawn(IEntitySource source)
        {
            SkewMultiplier = (Main.rand.NextBool() ? 1f : -1f) * Main.rand.NextFloat(0.4f);
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(SkewMultiplier * 0.05f);
            {
                Particle.Create<TintableSpark>(particle =>
                {
                    particle.Position = Projectile.Center;
                    particle.Scale = new(Main.rand.NextFloat(2f, 6f));
                    particle.ScaleVelocity = new Vector2(-35f);
                    particle.Velocity = Projectile.velocity.RotatedBy(MathHelper.Pi + Main.rand.NextFloatDirection() * 0.2f) * 0.2f;
                    particle.Color = new Color(30, 255, 105, 255) * (1f - Projectile.alpha / 255f);
                });
            }

            Lighting.AddLight(Projectile.Center, new Color(30, 255, 105).ToVector3());

            Projectile.alpha -= 5;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 10; i++)
            {
                Particle.Create<TintableSpark>(particle =>
                {
                    particle.Position = Projectile.Center;
                    particle.Scale = new(Main.rand.NextFloat(2f, 6f));
                    particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 6f);
                    particle.Color = new Color(30, 255, 105, 255);
                });
            }

            Lighting.AddLight(Projectile.Center, new Color(30, 255, 105).ToVector3() * 1.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var state = Main.spriteBatch.SaveState();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            var strip = new VertexStrip();

            GameShaders.Misc["MagicMissile"]
                .UseProjectionMatrix(true)
                .UseImage0(TextureAssets.MagicPixel)
                .UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Spark7"))
                .UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Spark6"))
                .Apply();

            var positions = (Vector2[])Projectile.oldPos.Clone();
            for (int i = 1; i < positions.Length; i++)
            {
                if (positions[i] == default)
                    continue;

                positions[i] += Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f);
            }

            strip.PrepareStripWithProceduralPadding(
                positions[1..],
                Projectile.oldRot[1..],
                _ => new Color(30, 255, 105) * (1f - Projectile.alpha / 255f),
                progress => visualScale * 30f * (1f - MathF.Pow(2f * progress - 1, 2)),
                Projectile.Size * 0.5f - Main.screenPosition,
                false,
                true
            );

            strip.DrawTrail();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return false;
        }
    }
}
