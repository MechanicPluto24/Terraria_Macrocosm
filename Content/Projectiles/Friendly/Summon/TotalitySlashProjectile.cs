using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class TotalitySlashProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
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
            Projectile.timeLeft = 360;
            Projectile.friendly = false;
        }

        private bool spawned;
        private Color color;

        public override void AI()
        {
            if (!spawned)
            {
                this.color = Main.rand.Next(3) switch
                {
                    0 => new(44, 210, 91),
                    1 => new(201, 125, 205),
                    _ => new(114, 111, 207),
                };
                spawned = true;
            }

            AI_Timer++;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Color color = Projectile.frame switch
            {
                0 => new(44, 210, 91),
                1 => new(201, 125, 205),
                _ => new(114, 111, 207),
            };


            // TODO: somehow preserve trail data after this point
            if (AI_Timer > 40)
            {
                Projectile.velocity *= 0.96f;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(-MathHelper.Pi / 80);

            Projectile.Opacity = Projectile.timeLeft / 360f;

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
            Vector2 origin = texture.Size() / 2f;

            for (int i = ProjectileID.Sets.TrailCacheLength[Type]; i > 1; i--)
            {
                if (i >= Projectile.oldPos.Length)
                    continue;

                Color color = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

                Vector2 oldPosition = Projectile.oldPos[i];

                float rotation = Projectile.oldRot[i] + MathHelper.PiOver2;
                SpriteEffects effects = ((Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                if (oldPosition == Vector2.Zero)
                    continue;

                Vector2 position = oldPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                float scale = MathHelper.Clamp((0.7f - i / (float)ProjectileID.Sets.TrailCacheLength[Type]), 0, 1);
                Main.EntitySpriteDraw(texture, position, null, color.WithAlpha(0), rotation, origin, scale, effects);
            }


            for (int i = ProjectileID.Sets.TrailCacheLength[Type]; i > 0; i--)
            {
                if (i >= Projectile.oldPos.Length)
                    continue;

                Color color = Color.Black.WithAlpha(200) * Projectile.Opacity;

                Vector2 oldPosition = Projectile.oldPos[i];

                float rotation = Projectile.oldRot[i] + MathHelper.PiOver2;
                SpriteEffects effects = ((Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                if (oldPosition == Vector2.Zero)
                    continue;

                Vector2 position = oldPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                float scale = MathHelper.Clamp((0.5f - i / (float)ProjectileID.Sets.TrailCacheLength[Type]), 0, 1);
                Main.EntitySpriteDraw(texture, position, null, color, rotation, origin, scale, effects);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => color.WithAlpha(200);
    }
}
