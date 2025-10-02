using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova;

public class HaemonovaLamp : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteLamp>(), (int)LuminiteStyle.Haemonova);
        Item.width = 10;
        Item.height = 32;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<HaemonovaBrick>(3)
            .AddIngredient<LunarCrystal>(1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
