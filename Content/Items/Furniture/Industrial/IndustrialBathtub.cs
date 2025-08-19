using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Industrial;

public class IndustrialBathtub : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialBathtub>());
        Item.width = 30;
        Item.height = 20;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<IndustrialPlating>(14)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
    }
}
