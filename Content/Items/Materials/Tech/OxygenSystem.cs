using Macrocosm.Content.Items.Materials.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class OxygenSystem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tech.OxygenSystem>());
            Item.width = 26;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
            Item.material = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OxygenTank>(2)
                .AddIngredient<PrintedCircuitBoard>()
                .AddIngredient<Plastic>(3)
                .AddIngredient(ItemID.CopperBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();

            CreateRecipe()
                .AddIngredient<OxygenTank>(2)
                .AddIngredient<PrintedCircuitBoard>()
                .AddIngredient<Plastic>(3)
                .AddIngredient(ItemID.TinBar, 10)
                .AddTile<Tiles.Crafting.Fabricator>() 
                .Register();
        }
    }
}