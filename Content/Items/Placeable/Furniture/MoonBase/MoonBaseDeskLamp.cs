using Macrocosm.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
    public class MoonBaseDeskLamp : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseDeskLamp>());
            Item.width = 32;
            Item.height = 30;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<MoonBasePlating>(5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
