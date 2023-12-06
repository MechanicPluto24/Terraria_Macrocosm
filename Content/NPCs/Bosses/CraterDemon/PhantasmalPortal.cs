using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    public class PhantasmalPortal : ModProjectile
    {
        public ref float AITimer => ref Projectile.ai[0];

        public bool Phase2
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int TargetPlayer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public int SpawnPeriod => 60;

        protected int defWidth;
        protected int defHeight;

        protected bool spawned;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            defWidth = defHeight = Projectile.width = Projectile.height = 52;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = CraterDemon.PortalTimerMax;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                AITimer = 255f;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item78, Projectile.Center);
            }

            Projectile.rotation -= MathHelper.ToRadians(7.4f);

            if (Projectile.timeLeft >= CraterDemon.PortalTimerMax - 90)
                AITimer -= 2.83333325f;
            else if (Projectile.timeLeft <= 90)
                AITimer += 2.83333325f;
            else
            {
                AITimer = 0f;
                if (Projectile.timeLeft % SpawnPeriod == 0)
                {
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 velocity = (Main.player[TargetPlayer].Center - Projectile.Center).SafeNormalize(Vector2.One).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(12, 20);
                            int type = ModContent.ProjectileType<PhantasmalImpSmall>();
                            int damage = Projectile.damage;

                            if (Main.rand.NextBool(3))
                            {
                                type = ModContent.ProjectileType<PhantasmalImp>();
                                velocity *= 0.8f;
                                damage = (int)(damage * 1.4f);
                            }

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, damage, Projectile.knockBack, Projectile.owner, TargetPlayer);
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.AbigailAttack, Projectile.Center);
                        }
                    }

                }
            }

            for (int i = 0; i < 20; i++)
            {
                float progress = (1f - AITimer / 255f);
                Particle.CreateParticle<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 2.2f * progress;
                    p.Velocity = Vector2.One * 8;
                    p.Scale = (0.1f + Main.rand.NextFloat(0.1f)) * progress;
                    p.Color = new Color(92, 206, 130);
                    p.TargetCenter = Projectile.Center;
                });
            }

            Projectile.alpha = (int)MathHelper.Clamp((int)AITimer, 0f, 255f);

            Vector2 center = Projectile.Center;
            Projectile.scale = 0.05f + 0.95f * (1f - Projectile.alpha / 255f);
            Projectile.width = (int)(defWidth * Projectile.scale);
            Projectile.height = (int)(defHeight * Projectile.scale);
            Projectile.Center = center;
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

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 1f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 1.4f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 0.84f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 1f, (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare3").Value;
            float scale = Projectile.scale * Main.rand.NextFloat(0.9f, 1.1f);
            Main.spriteBatch.Draw(flare, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(30, 255, 105).WithOpacity(0.75f), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            return false;
        }
    }
}
