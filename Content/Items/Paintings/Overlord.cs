using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class Overlord : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Overlord>());
        Item.width = 32;
        Item.height = 32;
    }

    public override void AddRecipes()
    {

    }
}