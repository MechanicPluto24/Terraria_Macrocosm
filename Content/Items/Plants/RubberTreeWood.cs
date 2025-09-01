using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Plants;

public class RubberTreeWood : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        //Item.DefaultToPlaceableTile(ModContent.TileType<RubberWood>());
        Item.DefaultToPlaceableTile(TileID.RichMahogany);
        Item.width = 8;
        Item.height = 10;
    }
}
