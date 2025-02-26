using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    public class IrradiatedAltar : ModItem
    {
        public override void SetStaticDefaults()
        {
            //ItemSets.DeveloperItem[Type] = true;
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Misc.IrradiatedAltar>());
            Item.width = 36;
            Item.height = 24;
            Item.value = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll((line) => line.Name == "Placeable");
        }
    }
}