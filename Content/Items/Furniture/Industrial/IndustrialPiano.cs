using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialPiano : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialPiano>());
            Item.width = 30;
            Item.height = 28;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IndustrialPlating>(15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient<PrintedCircuitBoard>()
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}
