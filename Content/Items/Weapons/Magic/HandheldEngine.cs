using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class HandheldEngine : ModItem
    {
        public override void SetStaticDefaults()
        {

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
            Item.shoot = ModContent.ProjectileType<HandheldEngineProjectile>();
            Item.shootSpeed = 1f;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 150;
            Item.reuseDelay = 8;
            Item.mana = 5;
        }

        public override bool CanUseItem(Player player) =>
            player.ownedProjectileCounts[Item.shoot] <= 0 &&
            !player.HasBuff<HandheldEngineOverheat>();

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
