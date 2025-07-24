using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial;

public class IndustrialPlatform : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 200;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialPlatform>());
        Item.width = 24;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient<IndustrialPlating>()
            .Register();
    }
}