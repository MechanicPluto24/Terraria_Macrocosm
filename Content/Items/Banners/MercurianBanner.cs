using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Banners;

public class MercurianBanner : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemSets.UnobtainableItem[Type] = true;
        Utility.AddVariationToRubblemakers(Type, ModContent.TileType<Tiles.Banners.MercurianBanner>(), 0);
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Banners.MercurianBanner>(), 1);
        Item.width = 24;
        Item.height = 36;
        Item.value = 500;
    }
}