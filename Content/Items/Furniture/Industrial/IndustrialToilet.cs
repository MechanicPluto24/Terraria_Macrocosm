using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial;

public class IndustrialToilet : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialToilet>());
        Item.width = 16;
        Item.height = 26;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<IndustrialPlating>(6)
            .Register();
    }
}
