using Macrocosm.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
    public class MoonBaseLantern : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseLantern>());
            Item.width = 16;
            Item.height = 34;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<MoonBasePlating>(3)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
