using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class DianitePortal : ModProjectile
    {

        protected int defWidth;
        protected int defHeight;

        protected bool manaCheck = true;

        public int AITimer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int AIShootTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
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
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || !player.channel || !manaCheck)
            {
                Projectile.Kill();
                return;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);

            Projectile.rotation += MathHelper.ToRadians(7.4f) * player.direction;

            AITimer++;

            if (AITimer > 12 && AITimer % 10 == 0)
            {
                manaCheck = player.CheckMana(3, true);

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 target = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                    int direction = Math.Sign(target.X);

                    float shootAngle = -MathHelper.Pi/12 * direction * MathF.Abs(MathF.Cos(target.ToRotation()));
                    float shootSpeed = Main.rand.NextFloat(16f, 20f);

                    Vector2 shootVelocity = target.RotatedBy(shootAngle) * shootSpeed;
                    Vector2 shootPosition = Projectile.Center + Main.rand.NextVector2Circular(30, 30);

                    int damage = Projectile.damage;
                    int type = ModContent.ProjectileType<DianiteMeteorSmall>();

                    if (Main.rand.NextBool(4))
                    {
                        damage = (int)(damage * 1.2f);
                        type = ModContent.ProjectileType<DianiteMeteor>();
                    }
       
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), shootPosition, shootVelocity.RotatedByRandom(MathHelper.Pi/24), type, damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
                }
            }

            if(AITimer % 16 == 0)
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

            Projectile.alpha = (int)MathHelper.Clamp((int)(255 - Projectile.ai[0]/12f * 255f), 0f, 255f);
            Vector2 center = Projectile.Center;
            Projectile.scale = 0.05f + 0.65f * (1f - Projectile.alpha / 255f);
            Projectile.width = (int)(defWidth * Projectile.scale);
            Projectile.height = (int)(defHeight * Projectile.scale);
            Projectile.Center = center;

            Lighting.AddLight(Projectile.Center, new Color(255, 170, 33).ToVector3() * 5f * Projectile.scale);
            SpawnParticles(10);
        }

        private void SpawnParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                float progress = (1f - Projectile.ai[0] / 255f);
                Particle.CreateParticle<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 1.6f * progress;
                    p.Velocity = Vector2.One * 8;
                    p.Scale = (0.1f + Main.rand.NextFloat(0.1f)) * progress;
                    p.Color = new Color(255, 170, 33) * 0.4f;
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
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color = Color.White;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 0.4f), (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 1.8f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color).WithOpacity(0.9f), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 1f).WithOpacity(1f), Projectile.rotation * 1.65f, texture.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare2").Value;
            float scale = Projectile.scale * Main.rand.NextFloat(0.85f, 1.15f);
            Main.spriteBatch.Draw(flare, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(255, 170, 33).WithOpacity(0.35f), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}