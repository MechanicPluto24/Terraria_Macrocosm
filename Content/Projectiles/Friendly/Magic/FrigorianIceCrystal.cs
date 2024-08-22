using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianIceCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.velocity.Y += 0.5f * MacrocosmSubworld.CurrentGravityMultiplier;

            if(Projectile.timeLeft < 50 && Projectile.alpha < 255)
                Projectile.alpha += 5;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = (Math.Abs(Projectile.velocity.X) < 1f ? -oldVelocity.X : oldVelocity.X) * 0.6f;
            Projectile.velocity.Y = -Projectile.velocity.Y * 0.6f;

            if(Projectile.velocity.LengthSquared() > 1f)
                for (int i = 0; i < (int)10; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                    dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                    dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                    dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                    dust.noGravity = true;
                }

            return false;
        }
    }
}
