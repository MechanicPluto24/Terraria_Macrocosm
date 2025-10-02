using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.StarRoyale;

public class StarRoyaleWorkbench : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteWorkbench>(), (int)LuminiteStyle.StarRoyale);
        Item.width = 28;
        Item.height = 16;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.StarRoyaleBrick, 10)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
