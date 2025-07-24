using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class Empress : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Empress>());
        Item.width = 22;
        Item.height = 32;
    }

    public override void AddRecipes()
    {

    }
}