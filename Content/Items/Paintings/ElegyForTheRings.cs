using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class ElegyForTheRings : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.ElegyForTheRings>());
        Item.width = 50;
        Item.height = 34;
    }

    public override void AddRecipes()
    {

    }
}