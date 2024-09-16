using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseDeskMonitor")]
    public class IndustrialDeskMonitor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialDeskMonitor>(), 1);
            Item.width = 32;
            Item.height = 28;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(8)
            .AddIngredient<PrintedCircuitBoard>()
            .AddIngredient(ItemID.Glass)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}
