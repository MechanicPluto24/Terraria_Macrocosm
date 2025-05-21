using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class LockOnStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<LockOnStaffProjectile>(), 18, 16f, true);

            Item.damage = 2000;
            Item.knockBack = 4;
            Item.mana = 10;

            Item.width = 44;
            Item.height = 44;

            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity1>();

            Item.channel = true;
            Item.UseSound = null;

            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
