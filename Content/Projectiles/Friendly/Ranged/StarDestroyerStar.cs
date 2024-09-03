using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public abstract class StarDestroyerStar : ModProjectile
    {
        protected int timer = 0;
        protected Color color = new(0, 0, 0);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
        }
    }

    public class StarDestroyerStar_Blue : StarDestroyerStar
    {
        public override string Texture => base.Texture.Replace("_Blue", "");

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.SetTrail<StarTrail>();
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            //example mod
            NPC closestNPC = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];

                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            color = new Color(100, 100, 255);

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            NPC closestNPC = FindClosestNPC(9000f);

            if (closestNPC != null)
            {
                Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = Projectile.velocity + (vel * 0.9f);
                Projectile.velocity = (Projectile.velocity).SafeNormalize(Vector2.UnitX);
                Projectile.velocity *= 30f;
            }

            timer++;
            if (timer % Main.rand.Next(4, 7) == 0)
            {
                // 16 and 17 are the star gores
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, -Projectile.velocity.RotatedByRandom(0.4) * 0.1f, 17);
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (color) * 0.4f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 1.8f, SpriteEffects.None, 0);

            Vector2 spinningPoint = new Vector2(0f, -4f);
            Texture2D aura = TextureAssets.Extra[91].Value;
            Vector2 auraOrigin = new Vector2((float)aura.Width / 2f, 10f);
            Vector2 drawPosition = Projectile.Center + Projectile.velocity - Projectile.velocity * 0.5f;
            Vector2 offset = new Vector2(0f, Projectile.gfxOffY);
            float rotation = (float)Main.timeForVisualEffects / 60f;
            float scale = 0f;
            Color auraColor = (color * 0.4f).WithAlpha(0) * Projectile.Opacity;
            float angle = Projectile.velocity.ToRotation();
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.5f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + (float)Math.PI * 2f / 3f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.1f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + 4.1887903f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.3f + scale, SpriteEffects.None);
            
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Pink, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }

            for (float i = 0f; i < 2f; i += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
            }

            for (float i = 0f; i < 2f; i += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
        }
    }
    public class StarDestroyerStar_Yellow : StarDestroyerStar
    {
        public override string Texture => base.Texture.Replace("_Yellow", "");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.CritChance += 2;
            Projectile.SetTrail<StarTrailAlt>();
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f;
            color = new Color(255, 180, 25);

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            timer++;

            if (timer % Main.rand.Next(4, 7) == 0)
            {
                // 16 and 17 are the star gores
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity.RotatedByRandom(0.4) * 0.1f, 16); 
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (color) * 0.4f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 1.8f, SpriteEffects.None, 0);

            Vector2 spinningPoint = new Vector2(0f, -4f);
            Texture2D aura = TextureAssets.Extra[91].Value;
            Vector2 auraOrigin = new Vector2((float)aura.Width / 2f, 10f);
            Vector2 drawPosition = Projectile.Center + Projectile.velocity - Projectile.velocity * 0.5f;
            Vector2 offset = new Vector2(0f, Projectile.gfxOffY);
            float rotation = (float)Main.timeForVisualEffects / 60f;
            float scale = 0f;
            Color auraColor = (color * 0.4f).WithAlpha(0) * Projectile.Opacity;
            float angle = Projectile.velocity.ToRotation();
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.5f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + (float)Math.PI * 2f / 3f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.1f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + 4.1887903f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.3f + scale, SpriteEffects.None);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Pink, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }

            for (float i = 0f; i < 2f; i += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue).noGravity = true;
            }

            for (float i = 0f; i < 2f; i += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
        }
    }

}

