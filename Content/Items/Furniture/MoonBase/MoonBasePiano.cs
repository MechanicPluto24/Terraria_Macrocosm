using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Materials.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBasePiano : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBasePiano>());
            Item.width = 30;
            Item.height = 28;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MoonBasePlating>(15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient<PrintedCircuitBoard>()
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}
