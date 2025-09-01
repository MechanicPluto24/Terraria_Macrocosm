using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class WeCanDoItYellow : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.WeCanDoIt>(), tileStyleToPlace: 1);
        Item.width = 20;
        Item.height = 30;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
    }
}