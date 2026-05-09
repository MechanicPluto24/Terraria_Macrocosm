using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Tools;

public class IndustrialBobber : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = ProjAIStyleID.Bobber;
        Projectile.bobber = true;
        Projectile.penetrate = -1;
        Projectile.netImportant = true;
        DrawOriginOffsetY = -8;
    }
}