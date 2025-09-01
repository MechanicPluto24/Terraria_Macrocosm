using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LichBolt : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        // Similar to the luminite star.
        private Color colour = new(34, 221, 151, 255);

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 4f, 0.1f, colour.WithAlpha(0), Color.Transparent);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Utility.DrawStar(Projectile.Center - Main.screenPosition, 2, colour, 0.4f, Projectile.rotation, entity: true);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
