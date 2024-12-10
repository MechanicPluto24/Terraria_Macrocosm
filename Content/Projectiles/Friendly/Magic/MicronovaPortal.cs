using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class MicronovaPortal : ModProjectile
    {
        private static Asset<Texture2D> twirl;
        private static Asset<Texture2D> twirl2;
        private static Asset<Texture2D> glow;

        protected int defWidth;
        protected int defHeight;

        private bool spawned;
        private Vector2 shootAim;

        public int AITimer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool RandomAim
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : -1f;
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
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (!spawned)
            {
                Vector2 target = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                shootAim = RandomAim ? target.RotatedByRandom(MathHelper.Pi / 24) : target;
                spawned = true;
            }

            Player player = Main.player[Projectile.owner];
            Projectile.rotation += MathHelper.ToRadians(24f);
            Projectile.velocity *= 0f;
            AITimer++;

            if (AITimer == 20)
            {
                int damage = Projectile.damage;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + shootAim, shootAim, ModContent.ProjectileType<MicronovaBeam>(), damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI, ai1: shootAim.X, ai2: shootAim.Y);
                SoundEngine.PlaySound(SoundID.Item117, Projectile.Center);
            }

            if (AITimer % 16 == 0)
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

            Projectile.alpha = (int)MathHelper.Clamp((int)(255 - (Projectile.ai[0] / 20f) * 255f), 0f, 255f);
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
            for (int i = 0; i < count; i++)
            {
                float progress = (1f - Projectile.alpha / 255f);
                Particle.Create<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(80, 160).RotatedBy(shootAim.ToRotation() + MathHelper.PiOver4) * 0.4f * progress;
                    p.Velocity = Vector2.One * 14;
                    p.Scale = new((0.1f + Main.rand.NextFloat(0.1f)) * progress);
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
            SpawnParticles(150);
        }

        public override Color? GetAlpha(Color lightColor)
            => Color.White * (1f - Projectile.alpha / 255f);

        private static Asset<Effect> skew;
        private SpriteBatchState state;
        public override void PostDraw(Color lightColor)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color = Color.White * Projectile.Opacity;

            skew ??= ModContent.Request<Effect>(Macrocosm.ShadersPath + "Skew", AssetRequestMode.ImmediateLoad);
            Effect effect = skew.Value;
            effect.Parameters["uScale"].SetValue(0.6f);
            effect.Parameters["uRotation"].SetValue(Projectile.rotation * 2);

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, effect, state);

            twirl ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl3");
            twirl2 ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Twirl1");
            glow ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle5");

            Main.EntitySpriteDraw(twirl.Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(100, 170, 200).WithOpacity(1f), shootAim.ToRotation() + MathHelper.PiOver2, twirl.Size() / 2f, Projectile.scale * 0.305f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow.Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(0, 170, 200).WithOpacity(0.8f), shootAim.ToRotation() + MathHelper.PiOver2, glow.Size() / 2f, Projectile.scale * 0.785f, SpriteEffects.None, 0f);

            effect = skew.Value;
            effect.Parameters["uScale"].SetValue(0.6f);
            effect.Parameters["uRotation"].SetValue(Projectile.rotation);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, effect, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color).WithOpacity(0.9f * Projectile.Opacity), shootAim.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}