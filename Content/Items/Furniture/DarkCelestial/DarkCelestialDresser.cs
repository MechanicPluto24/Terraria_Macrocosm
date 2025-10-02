using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.DarkCelestial;

public class DarkCelestialDresser : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteDresser>(), (int)LuminiteStyle.DarkCelestial);
        Item.width = 36;
        Item.height = 22;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DarkCelestialBrick, 16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
