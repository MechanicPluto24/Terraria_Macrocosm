using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons {
    public class ArtemiteGreatswordProjectile : ModProjectile {
        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Crescent Moon");
            DisplayName.SetDefault("Artemite Greatsword");
        }

        public override void SetDefaults() {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 56;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.light = 1f;
        }
    }
}