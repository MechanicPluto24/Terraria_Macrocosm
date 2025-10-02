using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova;

public class HaemonovaCandelabra : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteCandelabra>(), (int)LuminiteStyle.Haemonova);
        Item.width = 30;
        Item.height = 22;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<HaemonovaBrick>(5)
            .AddIngredient<LunarCrystal>(3)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
