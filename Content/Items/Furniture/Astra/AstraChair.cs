using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Astra;

public class AstraChair : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChair>(), (int)LuminiteStyle.Astra);
        Item.width = 16;
        Item.height = 32;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient(ItemID.AstraBrick, 5)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}