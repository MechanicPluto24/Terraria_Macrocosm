using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArmstrongGauntletProjectile : ModProjectile
    {


        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.gfxOffY = -3f;
        }
        Vector2 dirVec;
        Vector2 armCenter;
        bool spawned = false;
        public ref float MaxTime => ref Projectile.ai[1];
        int Timer = 0;
        int seed = 0;
        public override void OnSpawn(IEntitySource source) => seed = Main.rand.Next(1, 99999999);
        Player Player => Main.player[Projectile.owner];
        bool Launch => Projectile.ai[2] == 1f;
        public override void AI()
        {
            if (Player.noItems || Player.dead || !Player.active)
                Projectile.Kill();
            if (!spawned)
            {
                armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 3, -3);
                dirVec = armCenter.DirectionTo(Main.MouseWorld);
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SparkleDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(0.3f, .6f), 0, Color.Lerp(Color.White, new Color(254, 228, 21), Main.rand.NextFloat()) * Main.rand.NextFloat(), Main.rand.NextFloat(0.07f, 0.1f));
                }
                if (Launch)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        //armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 3, -3);
                        //Projectile.Center = armCenter + (dirVec * -8f);
                        Player.velocity = dirVec * (Player.velocity.Length() + 10);
                    }
                }
                spawned = true;
            }

            //if (Timer % 3 == 0)
            //  Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SparkleDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f) * Main.rand.NextFloat(.1f, .4f), 0, Color.Lerp(Color.White, new Color(254, 228, 21), Main.rand.NextFloat()) * 0.5f, Main.rand.NextFloat(0.07f, 0.1f));
            Projectile.spriteDirection = Player.direction;

            armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 3, -3);
            Projectile.Center = armCenter;
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Launch)
                {
                    //dirVec = armCenter.DirectionTo(Main.MouseWorld);
                    //Projectile.Center = armCenter + dirVec * ((float)(Math.Sin(Timer * 0.4f) * 2f) - 12f);
                }
            }

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, dirVec.ToRotation() - MathHelper.PiOver2);
            Timer++;
            //if (Launch)
            //  Projectile.Center = armCenter + (dirVec * -8f);

            Projectile.rotation = dirVec.ToRotation();
            if (Timer >= MaxTime)
                Projectile.Kill();

            if (Launch)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SparkleDust>(), Vector2.Zero, 0, Color.Lerp(Color.White, new Color(254, 228, 21), Main.rand.NextFloat()) * 0.5f, Main.rand.NextFloat(0.07f, 0.1f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Particle.Create<ArmstrongHitEffect>((p) =>
                {
                    p.Position = target.Center;
                    p.Velocity = new Vector2(3f, 0).RotatedBy(Projectile.velocity.ToRotation());
                    p.Scale = new(1.2f);
                    p.Rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    p.StarPointCount = 1;
                    p.FadeInFactor = 1.2f;
                    p.FadeOutFactor = 0.7f;
                }, shouldSync: true
                );
            }
            if (Launch)
            {
                Particle.Create<PrettySparkle>((p) =>
                 {
                     p.Position = Projectile.Center;
                     p.Velocity = Vector2.Zero;
                     p.Color = new Color(100, 100, 150, 255);
                     p.Scale = new Vector2(4f, 4f);
                     p.Rotation = Projectile.rotation;
                     p.TimeToLive = 25;
                     p.DrawVerticalAxis = false;
                 });

                Player.velocity = dirVec * -Player.velocity.Length() * 0.8f;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            if (!Launch)
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (armCenter.DirectionTo(Main.MouseWorld) * ((float)(Math.Sin(Timer * 1f) * 20f) + 23f)), 5f * Projectile.scale, ref _);
            else
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + dirVec * 20f, 5f * Projectile.scale, ref _);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float prog = Utils.GetLerpValue(0, MaxTime, Timer);

            Vector2 drawPos = Projectile.Center;
            Vector2 offset = new Vector2(0f, Projectile.gfxOffY);
            //Main.EntitySpriteDraw(texture, drawPos + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            offset += Vector2.SmoothStep(-Projectile.velocity, Projectile.velocity * 0.5f, prog);

            Vector2 spinningPoint = new Vector2(0f, -3f);
            Texture2D aura = TextureAssets.Extra[91].Value;

            Texture2D aura2 = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "fireball").Value;

            Vector2 auraOrigin = new Vector2((float)aura.Width / 2f, 10f);

            Vector2 aura2Origin = new Vector2((float)aura2.Width / 2f, 10f);

            Vector2 drawPosition = Projectile.Center + (dirVec * ((float)(Math.Sin(Timer * 1f) * 20f) + 23f));

            float rotation = (float)Main.timeForVisualEffects / 40f;
            float scale = -0.5f;
            Color auraColor = new Color(100, 100, 100, 255) * 0.6f * ((float)(Math.Sin(Timer * 1f) * 0.5) + 0.5f);
            float angle = Projectile.velocity.ToRotation();
            float alpha = MathHelper.Lerp(1.4f, 0f, prog);

            UnifiedRandom rand = new UnifiedRandom(seed);

            //if (Launch)
            //  drawPosition = Projectile.Center;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation), null, auraColor * alpha, angle + (float)Math.PI / 2f, auraOrigin, 1.5f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura2, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation), null, auraColor * alpha, angle + (float)Math.PI / 2f, aura2Origin, 1.5f + scale, SpriteEffects.None);

            angle = Projectile.rotation + rand.NextFloat(-0.3f, 0.3f);

            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + 4.1887903f), null, new Color(254, 228, 21, 255) * alpha * 0.6f * ((float)(Math.Sin(Timer * 1f) * 0.5) + 0.5f), angle + (float)Math.PI / 2f, auraOrigin, 1.3f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura2, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + 4.1887903f), null, new Color(254, 228, 21, 255) * alpha * 0.6f * ((float)(Math.Sin(Timer * 1f) * 0.5) + 0.5f), angle + (float)Math.PI / 2f, aura2Origin, 1.3f + scale, SpriteEffects.None);

            for (int i = 0; i < 6; i++)
            {
                rand = new UnifiedRandom(seed + i);
                angle = Projectile.rotation + rand.NextFloat(-0.3f, 0.3f);

                float _alpha = MathHelper.Clamp(1 - 1 / 6 * i, 0.2f, 0.8f);
                Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + (float)Math.PI * 2f / 3f), null, auraColor * alpha * _alpha, angle + (float)Math.PI / 2f, auraOrigin, 0.7f + scale + (i * 0.2f), SpriteEffects.None);
                Main.EntitySpriteDraw(aura2, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + (float)Math.PI * 2f / 3f), null, auraColor * alpha * _alpha, angle + (float)Math.PI / 2f, aura2Origin, 0.7f + scale + (i * 0.2f), SpriteEffects.None);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
