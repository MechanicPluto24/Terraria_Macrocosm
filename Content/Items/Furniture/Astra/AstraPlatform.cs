using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Astra;

public class AstraPlatform : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 200;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminitePlatform>(), (int)LuminiteStyle.Astra);
        Item.width = 24;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient(ItemID.AstraBrick)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}