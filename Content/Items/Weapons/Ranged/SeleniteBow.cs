using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class SeleniteBow : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToBow(25, 20, true);

            Item.damage = 110;
            Item.knockBack = 4;

            Item.width = 32;
            Item.height = 54;

            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();

            Item.channel = true;
            Item.UseSound = SoundID.Item5;

            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + new Vector2(0, -7).RotatedBy(velocity.ToRotation()), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + new Vector2(0, +7).RotatedBy(velocity.ToRotation()), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SeleniteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
