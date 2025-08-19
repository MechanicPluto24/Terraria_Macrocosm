using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Environment;

public class MoonWormDust : ModProjectile
{
    public override string Texture => "Macrocosm/Content/Particles/RocketExhaustSmoke";

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void OnSpawn(IEntitySource source)
    {

        Projectile.frame = Main.rand.Next(3);
    }
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;

        Projectile.tileCollide = false;

        Projectile.timeLeft = 255;

        Projectile.netImportant = true;
    }
    public override void AI()
    {
        Projectile.scale *= 0.98f;
        Projectile.alpha++;
        if (Projectile.alpha >= 255)
        {
            Projectile.active = false;
        }

    }
}
