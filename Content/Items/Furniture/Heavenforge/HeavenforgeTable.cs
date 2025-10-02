using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Heavenforge;

public class HeavenforgeTable : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteTable>(), (int)LuminiteStyle.Heavenforge);
        Item.width = 32;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HeavenforgeBrick, 8)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
