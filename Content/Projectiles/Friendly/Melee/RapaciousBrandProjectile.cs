using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class RapaciousBrandProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 120;
        }

        bool spawned;
        float rotationSpeed;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!spawned)
            {
                rotationSpeed = 0.85f;
                Projectile.rotation += rotationSpeed;
                spawned = true;
            }

            Projectile.rotation += rotationSpeed;
            rotationSpeed *= 1f - 0.015f;
            Projectile.velocity *= 1f - 0.02f;

            if (WorldGen.SolidTile(Projectile.Center.ToTileCoordinates()))
            {
                rotationSpeed *= 1f - 0.01f;
                Projectile.velocity *= 1f - 0.08f;
            }

            var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f));
            dust.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 35; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Scale: Main.rand.NextFloat(1.2f, 1.8f));
                dust.velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5f, 10f);
                dust.noGravity = true;
            }
        }
    }
}