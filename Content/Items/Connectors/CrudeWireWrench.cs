using Macrocosm.Common.ID;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

/// <summary> Abstract base for single-color wire wrenches. </summary>
public abstract class CrudeWireWrench : ModItem
{
    /// <summary> WorldGen wire-placement method for this wrench's color. </summary>
    protected abstract Func<int, int, bool> PlaceWireFunc { get; }

    /// <summary> NetMessage action ID for this wire color (5 = red, 10 = blue). </summary>
    protected abstract int NetAction { get; }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.mech = true;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override bool CanUseItem(Player player)
        => player.ItemInTileReach(Item)
        && player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY)
        && FindWire(player) >= 0;

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        int slot = FindWire(player);
        if (slot < 0 || !PlaceWireFunc(Player.tileTargetX, Player.tileTargetY))
            return false;

        if (ItemLoader.ConsumeItem(player.inventory[slot], player))
            player.inventory[slot].stack--;

        if (player.inventory[slot].stack <= 0)
            player.inventory[slot].SetDefaults(0);

        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, NetAction, Player.tileTargetX, Player.tileTargetY); // NetAction = WireActionID.*
        return true;
    }

    private static int FindWire(Player player)
    {
        for (int i = 0; i < 58; i++)
            if (player.inventory[i].stack > 0 && player.inventory[i].type == ItemID.Wire)
                return i;
        return -1;
    }
}
