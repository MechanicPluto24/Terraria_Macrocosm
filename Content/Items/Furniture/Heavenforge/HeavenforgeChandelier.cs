using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Heavenforge;

public class HeavenforgeChandelier : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteChandelier>(), (int)LuminiteStyle.Heavenforge);
        Item.width = 30;
        Item.height = 20;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HeavenforgeBrick, 4)
            .AddIngredient<LunarCrystal>(4)
            .AddIngredient(ItemID.Chain, 1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
