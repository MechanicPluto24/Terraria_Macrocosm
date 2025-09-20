using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.DarkCelestial;

public class DarkCelestialClock : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteClock>(), (int)LuminiteStyle.DarkCelestial);
        Item.width = 20;
        Item.height = 40;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DarkCelestialBrick, 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 3)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
