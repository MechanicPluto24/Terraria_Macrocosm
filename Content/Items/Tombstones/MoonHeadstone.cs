using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tombstones
{
    public class MoonHeadstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            Utility.AddVariationToRubblemakers(Type, ModContent.TileType<Tiles.Tombstones.MoonTombstone>(), 3);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tombstones.MoonTombstone>(), 2);
            Item.width = 26;
            Item.height = 30;
            Item.rare = ItemRarityID.Purple;
        }
    }
}