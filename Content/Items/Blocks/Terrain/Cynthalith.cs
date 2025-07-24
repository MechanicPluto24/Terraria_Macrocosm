using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Terrain;

public class Cynthalith : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.Cynthalith>());
    }

    public override void AddRecipes()
    {
    }
}