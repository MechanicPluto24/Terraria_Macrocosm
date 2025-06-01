using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
    public class PhantasmalBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;

            Redemption.AddElement(Projectile, Redemption.ElementID.Shadow);
            Redemption.AddElement(Projectile, Redemption.ElementID.Celestial);
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = spawnTimeLeft;
            CooldownSlot = 1;
        }

        int defDamage;
        int spawnTimeLeft = 480;
        bool spawned = false;
        public override void AI()
        {
            if (!spawned)
            {
                // TODO: sound
                spawned = true;
                defDamage = Projectile.damage;
            }

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X) - MathHelper.PiOver2;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            }


            if (Projectile.timeLeft > spawnTimeLeft * 0.2f)
            {
                Projectile.alpha -= (byte)(Projectile.velocity.Length() * 0.7f);
            }
            else
            {
                Projectile.alpha += (byte)(Projectile.velocity.Length() * 0.5f);
                Projectile.damage -= (int)(defDamage * 0.1f);
            }

            Projectile.alpha = (byte)MathHelper.Clamp(Projectile.alpha, 0, 255);

            if (++Projectile.frameCounter >= 9)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.frame = 0;
            }
            return;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            float count = 28 * (1f - Projectile.alpha / 255f);
            for (int n = 0; n < count; n++)
            {
                float opacity = (1f - Projectile.alpha / 255f);
                float progress = n / count;
                Color color = Color.White * (0.7f - progress) * opacity;
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                int frameY = (Projectile.frame + n) % 5;
                Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: frameY);
                Vector2 trailPosition = Projectile.position - Projectile.velocity.SafeNormalize(default) * n * 6f;
                Main.EntitySpriteDraw(texture, trailPosition - Main.screenPosition, frame, color, Projectile.rotation, frame.Size() / 2f, Projectile.scale * 1f * (1f - progress), SpriteEffects.None, 0);
                Utility.DrawStar(trailPosition + new Vector2(0, -6).RotatedBy(Projectile.rotation) - Main.screenPosition, 1, new Color(31, 255, 106, 0) * opacity * 0.3f, 0.72f * (1f - progress), Projectile.rotation, entity: true);
            }
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White.WithOpacity(0.5f) * (1f - Projectile.alpha / 255f);

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
