using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Liquids;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items;

public class BucketGlobalItem : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.EmptyBucket;

    public override bool? UseItem(Item item, Player player)
    {
        if(player.whoAmI != Main.myPlayer)
            return null;

        Tile tile = Utility.TargetTile();
        if (tile.LiquidAmount == 0 || tile.LiquidType < LiquidID.Count)
            return null;

        if (tile.LiquidType == ModLiquidLib.ModLiquidLib.LiquidType<Oil>())
        {
            Utility.RemoveLiquid<Oil>(Player.tileTargetX, Player.tileTargetY);
            Utility.TransformItemAndPutInInventory(player, item, ModContent.ItemType<OilBucket>());
            return true;
        }
        else if (tile.LiquidType == ModLiquidLib.ModLiquidLib.LiquidType<RocketFuel>())
        {
            Utility.RemoveLiquid<RocketFuel>(Player.tileTargetX, Player.tileTargetY);
            Utility.TransformItemAndPutInInventory(player, item, ModContent.ItemType<RocketFuelBucket>());
            return true;
        }

        return null;
    }
}
