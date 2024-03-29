using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class MoonSwordProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;

            Projectile.SetTrail<MoonSwordTrail>();
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 6, frameY: Projectile.frame);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Projectile.GetTrail().Draw(new Vector2(24 * Projectile.direction, 4).RotatedBy(Projectile.rotation));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, frame, Color.White.WithOpacity(1f), Projectile.rotation, frame.Size() / 2f, Projectile.scale, effects);

            return false;
        }

        public override void AI()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            Projectile.Opacity = 0.75f;

            Projectile.rotation = MathHelper.WrapAngle(Projectile.velocity.ToRotation() - (Projectile.direction == 1 ? 0.2f : MathHelper.Pi - 0.2f));

            Projectile.spriteDirection = Projectile.direction;

            for (int i = 0; i < 5; i++)
                Particle.CreateParticle<ImbriumStar>(Projectile.position + Main.rand.NextVector2Circular(20, 55), -Projectile.velocity * 0.02f, 0.5f);

            int frameSpeed = 3;
            if (Projectile.frameCounter++ >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }
    }
}