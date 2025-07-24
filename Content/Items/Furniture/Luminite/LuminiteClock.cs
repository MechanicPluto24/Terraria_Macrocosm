using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Luminite;

public class LuminiteClock : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteClock>(), (int)LuminiteStyle.Luminite);
        Item.width = 20;
        Item.height = 40;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.LunarBrick, 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
