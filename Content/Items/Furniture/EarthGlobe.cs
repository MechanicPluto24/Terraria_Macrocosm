using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture;

public class EarthGlobe : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.EarthGlobe>());
        Item.width = 24;
        Item.height = 28;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<Plastic>(1)
        .AddIngredient<AluminumBar>(1)
        .AddTile(TileID.WorkBenches)
        .Register();
    }
}