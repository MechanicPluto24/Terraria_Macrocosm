using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tools;

public class MoonBeastBobber : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = 61;
        Projectile.bobber = true;
        Projectile.penetrate = -1;
        Projectile.netImportant = true;
        DrawOriginOffsetY = -8;
    }
}