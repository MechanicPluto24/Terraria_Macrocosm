using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Haemonova;

public class HaemonovaSofa : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteSofa>(), (int)LuminiteStyle.Haemonova);
        Item.width = 34;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<HaemonovaBrick>(5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
