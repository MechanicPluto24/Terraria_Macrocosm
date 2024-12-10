using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class Procellarum : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 72;
            Item.damage = 550;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Procellarum_HalberdProjectile>();
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.shootSpeed = 1f;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.AltFunction())
            {
                Item.damage = 280;
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.autoReuse = true;
                Item.channel = false;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            else
            {
                Item.damage = 550;
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.autoReuse = true;
                Item.channel = true;
                Item.useStyle = ItemUseStyleID.Shoot;
            }
            return player.ownedProjectileCounts[ModContent.ProjectileType<Procellarum_HalberdProjectile>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.mouseRight)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Procellarum_HalberdProjectile>(), damage, knockback, player.whoAmI, 2f);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Procellarum_HalberdProjectile>(), damage, knockback, player.whoAmI, 1f);
            }
            return false;
        }
    }
}
