using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.LunarRust;

public class LunarRustClock : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteClock>(), (int)LuminiteStyle.LunarRust);
        Item.width = 20;
        Item.height = 40;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.LunarRustBrick, 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
