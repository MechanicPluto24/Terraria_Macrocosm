using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Blocks.Bricks;

namespace Macrocosm.Content.Items.Furniture.Haemonova;

public class HaemonovaPlatform : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 200;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminitePlatform>(), (int)LuminiteStyle.Haemonova);
        Item.width = 24;
        Item.height = 16;
    }

    public override void AddRecipes()
    {
        CreateRecipe(2)
            .AddIngredient<HaemonovaBrick>(1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}