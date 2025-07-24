using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Torches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cheese;

public class CheeseLamp : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Cheese.CheeseLamp>());
        Item.width = 16;
        Item.height = 32;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<CheeseBlock>(3)
            .AddIngredient<LuminiteTorch>(1)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
