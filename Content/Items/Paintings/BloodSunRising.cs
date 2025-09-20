using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class BloodSunRising : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.BloodSunRising>());
        Item.width = 32;
        Item.height = 22;
    }

    public override void AddRecipes()
    {

    }
}