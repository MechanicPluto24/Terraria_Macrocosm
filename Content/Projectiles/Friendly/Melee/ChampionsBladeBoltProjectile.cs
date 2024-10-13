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
            if (Projectile.timeLeft % 8 == 0)
            {
                Particle.Create<LuminiteSpark>(particle =>
                {
                    particle.Position = Projectile.Center;
                    particle.Velocity = Projectile.velocity.RotatedBy(MathHelper.Pi + Main.rand.NextFloatDirection() * 0.2f) * 0.2f;
                });
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 10; i++)
            {
                Particle.Create<LuminiteSpark>(particle =>
                {
                    particle.Position = Projectile.Center;
                    particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 20f);
                });
            }
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
                {
                    continue;
                }

                positions[i] += Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f);
            }

            strip.PrepareStripWithProceduralPadding(
                positions[1..],
                Projectile.oldRot[1..],
                _ => Color.White,
                progress => visualScale * 30f * (1f - MathF.Pow(2f * progress - 1, 2)),
                Projectile.Size * 0.5f - Main.screenPosition,
                false,
                true
            );

            strip.DrawTrail();

            var starTexture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Spark2", AssetRequestMode.ImmediateLoad).Value;
            foreach (var position in Projectile.oldPos)
            {
                if (!Main.rand.NextBool(7))
                {
                    continue;
                }

                Main.spriteBatch.Draw(
                    starTexture,
                    position + Projectile.Size * 0.5f - Main.screenPosition + Main.rand.NextVector2Unit() * 6f,
                    null,
                    Color.White * Main.rand.NextFloat(0.2f, 0.7f),
                    Main.rand.NextFloat(),
                    starTexture.Size() * 0.5f,
                    Main.rand.NextFloat() * 0.09f,
                    SpriteEffects.None,
                    0f
                );
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            // Main.spriteBatch.Draw();
            return false;
        }
    }
}
