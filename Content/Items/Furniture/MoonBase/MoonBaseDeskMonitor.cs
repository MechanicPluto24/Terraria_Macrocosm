using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Materials.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBaseDeskMonitor : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseDeskMonitor>(), 1);
            Item.width = 32;
            Item.height = 28;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<MoonBasePlating>(8)
            .AddIngredient<PrintedCircuitBoard>()
            .AddIngredient(ItemID.Glass)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}
