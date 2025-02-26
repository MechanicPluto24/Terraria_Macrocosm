using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class RocheChakram : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;

        }

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.value = Item.sellPrice(silver: 4);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = true;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.damage = 265;
            Item.knockBack = 0.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<RocheChakramProjectile>();
            Item.maxStack = 9999;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RocheChakramProjectile>(), damage, knockback, Main.myPlayer, ai0: 0f);
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);

            return null;
        }

        public override void AddRecipes()
        {
            CreateRecipe(20)
            .AddIngredient<ArtemiteBar>(1)
            .AddIngredient(ItemID.LunarBar, 1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
