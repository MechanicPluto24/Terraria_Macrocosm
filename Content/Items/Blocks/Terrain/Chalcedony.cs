using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Terrain;

public class Chalcedony : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.Chalcedony>());
    }

    public override void AddRecipes()
    {
    }
}
