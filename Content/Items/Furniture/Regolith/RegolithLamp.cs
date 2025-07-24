using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Macrocosm.Content.Items.Torches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Regolith;

public class RegolithLamp : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithLamp>());
        Item.width = 12;
        Item.height = 34;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<RegolithBrick>(3)
            .AddIngredient<LuminiteTorch>(1)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
