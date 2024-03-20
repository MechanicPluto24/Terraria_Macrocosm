using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class ArtemiteSpearProjectileThrown : ModProjectile
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
            Projectile.width = 18;
            Projectile.height = 18;

            AIType = ProjectileID.WoodenArrowFriendly;

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?

            Projectile.penetrate = 1;  

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;

            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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
                    Vector2 position = target.Center + Main.rand.NextVector2CircularEdge(450, 450);
                    Vector2 velocity = new Vector2(25, 0).RotatedBy((target.Center - position).ToRotation());
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, velocity, Type, (int)(Projectile.damage * 0.5f), Projectile.knockBack, Main.myPlayer, ai0: 1f);
                }
            }

            Particle.CreateParticle<ArtemiteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = 1.2f;
                p.Rotation = Projectile.oldVelocity.ToRotation() + MathHelper.PiOver2;
                p.StarPointCount = 1;
            },  shouldSync: true
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
                    int type = Main.rand.NextBool() ? ModContent.DustType<ArtemiteBrightDust>() : ModContent.DustType<ArtemiteDust>();
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
                    int type = Main.rand.NextBool() ? ModContent.DustType<ArtemiteBrightDust>() : ModContent.DustType<ArtemiteDust>();
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                    dust.noGravity = true;
                }
            }   
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Color color = Phantom ? new Color(130, 220, 199, 0) * opacity : lightColor;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Color trailColor = color * trailFactor * 0.1f;
                SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Main.EntitySpriteDraw(texture, drawPos, null, trailColor, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            if(Phantom)
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale * 0.5f, SpriteEffects.None, 0);


            return false;
        }
    }
}
