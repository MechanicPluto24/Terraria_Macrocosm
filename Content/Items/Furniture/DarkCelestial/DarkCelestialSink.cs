using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.DarkCelestial;

public class DarkCelestialSink : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteSink>(), (int)LuminiteStyle.DarkCelestial);
        Item.width = 32;
        Item.height = 28;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DarkCelestialBrick, 6)
            .AddIngredient(ItemID.WaterBucket, 1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
