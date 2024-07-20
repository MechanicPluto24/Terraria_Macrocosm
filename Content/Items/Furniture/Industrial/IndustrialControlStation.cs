using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Materials.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialControlStation : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialControlStation>(), 1);
            Item.width = 34;
            Item.height = 28;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(10)
            .AddIngredient<PrintedCircuitBoard>(2)
            .AddIngredient(ItemID.Glass, 4)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}