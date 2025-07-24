using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings;

public class ShoresOfOblivion : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.ShoresOfOblivion>());
        Item.width = 50;
        Item.height = 34;
    }

    public override void AddRecipes()
    {
    }
}