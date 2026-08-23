using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks;

public class IndustrialPlatingBroken : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.IndustrialPlatingBroken>());
    }
}
