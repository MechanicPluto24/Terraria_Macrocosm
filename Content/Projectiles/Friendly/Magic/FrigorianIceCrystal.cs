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
            Projectile.width = 30;
            Projectile.height = 36;
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
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.6f;

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                Projectile.velocity.Y = oldVelocity.Y * -0.6f;

            for (int i = 0; i < (int)(Projectile.velocity.Y / 4f); i++)
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
