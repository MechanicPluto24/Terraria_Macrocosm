using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.LunarRust;

public class LunarRustLamp : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteLamp>(), (int)LuminiteStyle.LunarRust);
        Item.width = 10;
        Item.height = 32;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.LunarRustBrick, 3)
            .AddIngredient<LunarCrystal>(1)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
