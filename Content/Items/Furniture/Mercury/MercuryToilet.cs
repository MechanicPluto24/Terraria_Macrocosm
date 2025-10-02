using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Mercury;

public class MercuryToilet : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteToilet>(), (int)LuminiteStyle.Mercury);
        Item.width = 16;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MercuryBrick, 6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
