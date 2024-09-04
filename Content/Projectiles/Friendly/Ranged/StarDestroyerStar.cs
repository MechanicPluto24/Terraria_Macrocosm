using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class StarDestroyerStar : ModProjectile
    {
        public enum StarVariant { Blue, Yellow }
        public StarVariant StarType
        {
            get => (Projectile.ai[0] == 0f ? StarVariant.Blue : StarVariant.Yellow);
            set => Projectile.ai[0] = (float)value;
        }

        public ref float AI_Timer => ref Projectile.ai[1];

        private Color color;
        private StarTrail trail;

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

        private bool spawned;
        public override void AI()
        {
            if (!spawned)
            {
                if (StarType is StarVariant.Blue)
                {
                    color = new Color(100, 100, 255);
                }
                else if (StarType is StarVariant.Yellow)
                {
                    color = new Color(255, 180, 25);
                    Projectile.CritChance += 2;
                }
                trail = new StarTrail { Color = color.WithAlpha(65) };
                spawned = true;
            }

            Projectile.rotation += 0.25f;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            AI_Timer++;

            if (StarType is StarVariant.Blue)
            {
                NPC closestNPC = Utility.GetClosestNPC(Projectile.Center, 9000f);
                if (closestNPC != null)
                {
                    Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.velocity = Projectile.velocity + (vel * 0.9f);
                    Projectile.velocity = (Projectile.velocity).SafeNormalize(Vector2.UnitX);
                    Projectile.velocity *= 30f;
                }
            }

            if (Projectile.alpha <= 0 && (int)(AI_Timer % Main.rand.Next(5, 9)) == 0)
            {
                int starGoreType = StarType is StarVariant.Blue ? 17 : 16; // 16 and 17 are the star gores
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, -Projectile.velocity.RotatedByRandom(0.4) * 0.1f, starGoreType);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            for (float i = 0f; i < 2f; i += 0.0125f)
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(i * ((float)Math.PI * 2f)) * Main.rand.NextFloat(8f), 0, color * 0.5f).noGravity = true;

            for (int i = 0; i < 8; i++)
            {
                int starGoreType = StarType is StarVariant.Blue ? 17 : 16; // 16 and 17 are the star gores
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(0, 2).RotatedByRandom(MathHelper.TwoPi), starGoreType);
            }
    }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = tex.Size() / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            trail.Draw(Projectile, Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (Color.White) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (color) * 0.4f * Projectile.Opacity, Projectile.rotation + MathHelper.Pi, origin, Projectile.scale * 1.6f, SpriteEffects.None, 0);

            Vector2 spinningPoint = new Vector2(0f, -12f);
            Texture2D aura = TextureAssets.Extra[91].Value;
            Vector2 auraOrigin = new Vector2((float)aura.Width / 2f, 10f);
            Vector2 drawPosition = Projectile.Center + Projectile.velocity - Projectile.velocity * 0.5f;
            Vector2 offset = new Vector2(0f, Projectile.gfxOffY);
            float rotation = (float)Main.timeForVisualEffects / 60f;
            float scale = 0f;
            Color auraColor = (color * 0.4f).WithAlpha(0) * Projectile.Opacity * 0.6f;
            float angle = Projectile.velocity.ToRotation();
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.5f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + (float)Math.PI * 2f / 3f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.1f + scale, SpriteEffects.None);
            Main.EntitySpriteDraw(aura, drawPosition - Main.screenPosition + offset + spinningPoint.RotatedBy((float)Math.PI * 2f * rotation + 4.1887903f), null, auraColor, angle + (float)Math.PI / 2f, auraOrigin, 1.3f + scale, SpriteEffects.None);

            return false;
        }
    }
}

