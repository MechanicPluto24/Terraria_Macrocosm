using Macrocosm.Common.Sets;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class LHBMinigun : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.UnobtainableItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LastPrism);
            Item.width = 74;
            Item.height = 32;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(silver: 700);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LHBMinigunProjectile>();
            Item.shootSpeed = 32f;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 35;
            Item.reuseDelay = 8;
            Item.useAmmo = AmmoID.Bullet;
            Item.mana = 0;
            Item.knockBack = 0.01f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] == 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
