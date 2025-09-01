using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class TotalitySmallSlashProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public ref float AI_Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.scale = 1f + (float)Main.rand.Next(30) * 0.01f;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 160;
            Projectile.friendly = false;
        }

        private bool spawned;

        public override void AI()
        {
            if (!spawned)
            {
                Projectile.frame = Main.rand.Next(3);
                spawned = true;
            }

            AI_Timer++;

            if (AI_Timer > 10)
                Projectile.friendly = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Color color = Projectile.frame switch
            {
                0 => new(44, 210, 91),
                1 => new(201, 125, 205),
                _ => new(114, 111, 207),
            };

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 64);
            Projectile.velocity *= 0.995f;

            Projectile.Opacity = Projectile.timeLeft / 160f;

            if (Main.rand.NextBool(8))
            {
                //Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<ElectricSparkDust>(), Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 0.9f);
                //dust.noGravity = true;
                //dust.position = Projectile.Center;
                //dust.velocity = Main.rand.NextVector2Circular(1f, 1f) + Projectile.velocity * 0.5f;
                //dust.color = color;
            }

            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.5f * Projectile.Opacity);

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frame = TextureAssets.Projectile[Projectile.type].Height() / Main.projFrames[Type];
            int frameHeight = frame * Projectile.frame;
            Rectangle sourceRect = new(0, frameHeight, texture.Width, frame);
            Vector2 origin = sourceRect.Size() / 2f;

            int start = ProjectileID.Sets.TrailCacheLength[Type];
            int end = 0;
            int incr = -2;

            for (int i = start; (incr > 0 && i < end) || (incr < 0 && i > end); i += incr)
            {
                if (i >= Projectile.oldPos.Length)
                    continue;

                Color color = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
                float diff = end - i;
                if (incr < 0)
                    diff = start - i;

                color *= diff / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                Vector2 oldPosition = Projectile.oldPos[i];

                float rotation = Projectile.oldRot[i] + MathHelper.PiOver2;
                SpriteEffects effects = ((Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                if (oldPosition == Vector2.Zero)
                    continue;

                Vector2 position = oldPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, position, sourceRect, color, rotation, origin, MathHelper.Lerp(Projectile.scale, 1f, i / 15f), effects);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);
    }
}
