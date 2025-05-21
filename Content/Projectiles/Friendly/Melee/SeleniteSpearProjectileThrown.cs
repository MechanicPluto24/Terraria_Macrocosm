using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class SeleniteSpearProjectileThrown : ModProjectile
    {
        public override string Texture => base.Texture.Replace("Thrown", "");

        public bool Phantom
        {
            get => Projectile.ai[0] > 0f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            AIType = ProjectileID.WoodenArrowFriendly;

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?

            Projectile.penetrate = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;

            // ??
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            MoRHelper.SetSpearBonus(Projectile);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Phantom)
            {
                fadeOut = true;
                Projectile.timeLeft = (int)(1f / 0.085f);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    float radius = 450f;
                    float speed = 25f;
                    Vector2 targetPosition = target.Center + target.velocity * (radius / speed);
                    Vector2 spawnPosition = targetPosition + Main.rand.NextVector2CircularEdge(radius, radius);
                    Vector2 velocity = new Vector2(speed, 0).RotatedBy((targetPosition - spawnPosition).ToRotation());
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPosition, velocity, Type, (int)(Projectile.damage * 0.5f), Projectile.knockBack, Main.myPlayer, ai0: 1f);
                }
            }

            Particle.Create<SeleniteStar>((p) =>
            {
                p.Position = Projectile.Center;
                p.Velocity = Projectile.velocity * 0.1f;
                p.Scale = new(1.2f);
                p.Rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                p.StarPointCount = 1;
                p.FadeInFactor = 1.8f;
                p.FadeOutFactor = 0.8f;
            }, shouldSync: true
            );
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }


        bool spawned = false;

        bool fadeOut;
        float opacity;

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;

                if (Phantom)
                {
                    Projectile.penetrate = -1;
                    Projectile.timeLeft = 25;
                    opacity = 0f;
                }
                else
                {
                    Projectile.penetrate = 1;
                    opacity = 1f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.PiOver4;

            if (Phantom)
            {
                Projectile.tileCollide = false;

                if (fadeOut)
                {
                    if (opacity > 0f)
                        opacity -= 0.085f;
                }
                else
                {
                    if (opacity < 1f)
                        opacity += 0.085f;
                }
            }
            else
            {
                Projectile.tileCollide = true;
                Projectile.velocity.Y += 0.12f;

                if (!Main.dedServ)
                {
                    int type = Main.rand.NextBool() ? ModContent.DustType<SeleniteBrightDust>() : ModContent.DustType<SeleniteDust>();
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X / -64f, Projectile.velocity.Y / -16f, Scale: 0.8f);
                    dust.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (!Phantom)
            {
                for (int i = 0; i < Main.rand.Next(30, 40); i++)
                {
                    int type = Main.rand.NextBool() ? ModContent.DustType<SeleniteBrightDust>() : ModContent.DustType<SeleniteDust>();
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                    dust.noGravity = true;
                    dust = Dust.NewDustDirect(Projectile.oldPos.ToList().GetRandom(), Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                    dust.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Particle.Create<SeleniteStar>((p) =>
            {
                p.Position = Projectile.Center - oldVelocity;
                p.Velocity = oldVelocity * 0.4f;
                p.Scale = new(1.2f);
                p.Rotation = oldVelocity.ToRotation() + MathHelper.PiOver2;
                p.StarPointCount = 1;
                p.FadeInFactor = 2f;
                p.FadeOutFactor = 0.85f;
            }, shouldSync: true
            );

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color = Phantom ? new Color(130, 220, 199, 0) * opacity : lightColor;

            if (!Phantom)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                    float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                    Color trailColor = color * trailFactor * 0.5f;
                    SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
                }

                Texture2D glow = TextureAssets.Extra[ExtrasID.SharpTears].Value;
                Main.EntitySpriteDraw(glow, Projectile.Center + new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition, null, new Color(130, 220, 199, 0), Projectile.rotation - MathHelper.PiOver4, glow.Size() / 2f, Projectile.scale, SpriteEffects.None);

                for (float progress = 0.4f; progress <= 1f; progress += 0.4f)
                    Main.EntitySpriteDraw(glow, Vector2.Lerp(Projectile.Center - Projectile.velocity, Projectile.Center, progress + 0.9f) - Main.screenPosition + new Vector2(0f, 0f), null, new Color(130, 220, 199, 0) * 0.75f * progress, Projectile.rotation - MathHelper.PiOver4, glow.Size() / 2f, new Vector2(0.8f, 2.2f) * Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
