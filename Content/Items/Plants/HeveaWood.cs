using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Blocks.Woods;

namespace Macrocosm.Content.Items.Plants;

public class HeveaWood : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Content.Tiles.Blocks.Woods.HeveaWood>());
        Item.width = 8;
        Item.height = 10;
    }
}
