using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture;

public class Boombox : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Boombox>());
        Item.width = 38;
        Item.height = 32;
        Item.value = 500;
    }
}