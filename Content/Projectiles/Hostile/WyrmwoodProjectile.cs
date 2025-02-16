using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Hostile
{
    public class WyrmwoodProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;

        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 14;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;

            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += MacrocosmSubworld.GetGravityMultiplier() / 4;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CoalDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
