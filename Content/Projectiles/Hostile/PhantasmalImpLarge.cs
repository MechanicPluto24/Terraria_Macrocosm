using Terraria;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class PhantasmalImpLarge : PhantasmalImpSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 66;
            Projectile.height = 74;
        }
    }
}
