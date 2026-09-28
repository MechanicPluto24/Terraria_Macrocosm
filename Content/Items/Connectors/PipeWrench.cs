using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

public abstract class PipeWrench : ModItem
{
    public abstract PipeType PipeType { get; }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.value = Item.buyPrice(silver: 10);
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.mech = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ItemInTileReach(Item)
            && player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY)
            && PickPipe(player, out _);
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        if (!PickPipe(player, out int slot))
            return false;

        if (PipeSystem.PlacePipe(player.TargetCoords(), PipeType))
        {
            player.inventory[slot].DecreaseStack(1, player);
            return true;
        }

        return false;
    }

    private static bool PickPipe(Player player, out int slot)
    {
        slot = -1;
        int pipeType = ModContent.ItemType<Pipe>();

        for (int i = 55; i <= 58; i++)
        {
            if (player.inventory[i].type == pipeType && player.inventory[i].stack > 0)
            {
                slot = i;
                return true;
            }
        }

        for (int i = 0; i <= 54; i++)
        {
            if (player.inventory[i].type == pipeType && player.inventory[i].stack > 0)
            {
                slot = i;
                return true;
            }
        }

        return false;
    }
}
