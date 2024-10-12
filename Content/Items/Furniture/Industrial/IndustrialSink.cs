using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseSink")]
    public class IndustrialSink : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialSink>());
            Item.width = 26;
            Item.height = 34;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CheeseBlock>(6)
                .AddIngredient(ItemID.WaterBucket, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
