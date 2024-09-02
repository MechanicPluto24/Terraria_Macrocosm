using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianIceShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;

            NPC closestNPC = Utility.GetClosestNPC(Projectile.Center, 9000f);
            if (closestNPC is not null)
            {
                Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity += (vel * 0.9f);
                Projectile.velocity = (Projectile.velocity).SafeNormalize(Vector2.UnitX);
                Projectile.velocity *= 17f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                dust.velocity.X = Main.rand.Next(-70, 71) * 0.02f;
                dust.velocity.Y = Main.rand.Next(-70, 70) * 0.02f;
                dust.scale *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
