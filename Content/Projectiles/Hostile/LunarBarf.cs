using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LunarBarf : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;

        }



        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;

            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += MacrocosmSubworld.Current.GravityMultiplier / 4;
            Projectile.rotation = Projectile.velocity.ToRotation();

            int frameSpeed = 6;
            if (Projectile.frameCounter++ >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GreenBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
