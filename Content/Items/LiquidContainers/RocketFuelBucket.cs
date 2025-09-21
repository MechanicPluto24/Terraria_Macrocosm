using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers;

public class RocketFuelBucket : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemSets.LiquidContainerData[Type] = new LiquidContainerData(ModLiquidLib.ModLiquidLib.LiquidType<RocketFuel>(), 50, ItemID.EmptyBucket);
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.useAnimation = 10;
        Item.useTime = 15;
        Item.autoReuse = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer && player.ItemInTileReach(Item))
        {
            Utility.PlaceLiquid<RocketFuel>(Player.tileTargetX, Player.tileTargetY);
            Utility.TransformItemAndPutInInventory(player, Item, ItemID.EmptyBucket);
            return true;
        }

        return null;
    }
}
