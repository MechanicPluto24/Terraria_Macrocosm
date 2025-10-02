using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Heavenforge;

public class HeavenforgeDoor : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteDoorClosed>(), (int)LuminiteStyle.Heavenforge);
        Item.width = 16;
        Item.height = 16;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HeavenforgeBrick, 6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
