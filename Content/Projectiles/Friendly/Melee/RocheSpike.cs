using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class RocheSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            AIType = ProjectileID.WoodenArrowFriendly;

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?

            Projectile.penetrate = 3; // 3 max hits 

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.03f;

            if (!Main.dedServ)
            {
                int type = Main.rand.NextBool() ? ModContent.DustType<ArtemiteBrightDust>() : ModContent.DustType<ArtemiteDust>();
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X / -64f, Projectile.velocity.Y / -16f, Scale: 0.8f);
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(30, 40); i++)
            {
                int type = Main.rand.NextBool() ? ModContent.DustType<ArtemiteBrightDust>() : ModContent.DustType<ArtemiteDust>();
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
