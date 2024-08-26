using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class MicronovaPortal : ModProjectile
    {
        protected int defWidth;
        protected int defHeight;

        private bool spawned;
        private Vector2 shootAim;

        public int AITimer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            defWidth = defHeight = Projectile.width = Projectile.height = 68;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!spawned)
            {
                Vector2 target = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                shootAim = target.RotatedByRandom(MathHelper.Pi / 24);
                spawned = true;
            }

            Player player = Main.player[Projectile.owner];
            Projectile.rotation += MathHelper.ToRadians(7.4f);
            Projectile.velocity *= 0f;
            AITimer++;

            if (AITimer == 20)
            {
                int damage = Projectile.damage;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, shootAim, ModContent.ProjectileType<MicronovaBeam>(), damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
            }

            if (AITimer % 16 == 0)
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

            Projectile.alpha = (int)MathHelper.Clamp((int)(255 - (Projectile.ai[0] / 5f) * 255f), 0f, 255f);
            Vector2 center = Projectile.Center;
            Projectile.scale = 0.05f + 0.65f * (1f - Projectile.alpha / 255f);
            Projectile.width = (int)(defWidth * Projectile.scale);
            Projectile.height = (int)(defHeight * Projectile.scale);
            Projectile.Center = center;

            Lighting.AddLight(Projectile.Center, new Color(0, 170, 200).ToVector3() * 5f * Projectile.scale);
            SpawnParticles(5);
            if (AITimer > 60)
                Projectile.Kill();
        }

        private void SpawnParticles(int count)
        {
            return;
            for (int i = 0; i < count; i++)
            {
                float progress = (1f - Projectile.alpha / 255f);
                Particle.CreateParticle<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 1.6f * progress;
                    p.Velocity = Vector2.One * 18;
                    p.Scale = (0.1f + Main.rand.NextFloat(0.1f)) * progress;
                    p.Color = new Color(0, 170, 200) * 0.6f;
                    p.TargetCenter = Projectile.Center;
                });
            }
        }

        public override bool? CanHitNPC(NPC npc)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            SpawnParticles(100);
        }

        public override Color? GetAlpha(Color lightColor)
            => Color.White * (1f - Projectile.alpha / 255f);


        private SpriteBatchState state;
        public override void PostDraw(Color lightColor)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color = Color.White * Projectile.Opacity;

            Matrix matrix = GetSkewMatrix(Projectile.Center - Main.screenPosition, state.Matrix, shootAim);

            state.SaveState(Main.spriteBatch, continuous: true);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, state.DepthStencilState, state.RasterizerState, null, matrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 0.4f * Projectile.Opacity), (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 1.8f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color).WithOpacity(0.9f * Projectile.Opacity), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;
            float scale = Projectile.scale * Main.rand.NextFloat(1.15f, 1.35f);
            Main.spriteBatch.Draw(flare, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(0, 170, 200).WithOpacity(0.85f * Projectile.Opacity), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            return false;
        }

        private static Matrix GetSkewMatrix(Vector2 drawPosition, Matrix baseMatrix, Vector2 aim)
        {
            Vector2 rotation = Utils.ToRotationVector2(aim.ToRotation());
            float radiansX = rotation.Y;
            float radiansY = rotation.X;

            // TODO
            Matrix transformationMatrix =
                Matrix.CreateTranslation(-drawPosition.X, -drawPosition.Y, 0f) * // Translate to screen origin
                Matrix.CreateRotationY(radiansY) *                                // Apply Y skew
                Matrix.CreateTranslation(drawPosition.X, drawPosition.Y, 0f) *   // Translate back to original position
                Matrix.CreateScale(baseMatrix.M11, baseMatrix.M22, 0f);          // Apply scale

            return baseMatrix;   
        }

        private static Matrix GetBladesMatrix(Vector2 drawPosition, Matrix baseMatrix)
        {
            return baseMatrix;
        }

    }
}