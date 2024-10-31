using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech
{
    public class Battery : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LithiumOre>(10)
                .AddIngredient<Coal>(2)
                .AddIngredient(ItemID.TinBar, 4)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();

            CreateRecipe()
                .AddIngredient<LithiumOre>(10)
                .AddIngredient<Coal>(2)
                .AddIngredient(ItemID.CopperBar, 4)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}