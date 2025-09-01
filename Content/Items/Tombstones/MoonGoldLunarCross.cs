using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tombstones;

public class MoonGoldLunarCross : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 2;
        Utility.AddVariationToRubblemakers(Type, ModContent.TileType<Tiles.Tombstones.MoonGoldTombstone>(), 5);
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tombstones.MoonGoldTombstone>(), 4);
        Item.width = 20;
        Item.height = 32;
        Item.rare = ItemRarityID.Purple;
    }
}