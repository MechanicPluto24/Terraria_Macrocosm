using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial;

public class IndustrialBed : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialBed>());
        Item.width = 34;
        Item.height = 24;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<IndustrialPlating>(15)
        .AddIngredient(ItemID.Silk, 5)
        .AddTile<Tiles.Crafting.Fabricator>()
        .Register();
    }
}