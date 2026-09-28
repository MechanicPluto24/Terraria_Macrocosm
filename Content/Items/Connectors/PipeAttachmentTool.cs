using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors;

[LegacyName("ConveyorAttachmentTool")]
public class PipeAttachmentTool : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.value = Item.buyPrice(silver: 50);
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTurn = true;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = true;
        Item.mech = true;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool CanUseItem(Player player)
    {
        if (!player.ItemInTileReach(Item) || !player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY))
            return false;

        if (player.altFunctionUse == 2)
            return PipeSystem.HasAttachment(player.TargetCoords());

        return PickAttachment(player, out _);
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        if (player.altFunctionUse == 2)
            return PipeSystem.TryRotateAttachment(player.TargetCoords());

        if (!PickAttachment(player, out int slot))
            return false;

        bool isHopper = player.inventory[slot].type == ModContent.ItemType<Hopper>();
        if (PipeSystem.TryPlaceOrSwapAttachment(player.TargetCoords(), isHopper))
        {
            player.inventory[slot].DecreaseStack(1, player);
            return true;
        }

        return false;
    }

    private static bool PickAttachment(Player player, out int slot)
    {
        slot = -1;

        for (int i = 55; i <= 58; i++)
        {
            if (IsAttachmentItem(player.inventory[i]))
            {
                slot = i;
                return true;
            }
        }

        for (int i = 0; i <= 54; i++)
        {
            if (IsAttachmentItem(player.inventory[i]))
            {
                slot = i;
                return true;
            }
        }

        return false;
    }

    private static bool IsAttachmentItem(Item item)
    {
        return item is not null
            && item.stack > 0
            && (item.type == ModContent.ItemType<Hopper>() || item.type == ModContent.ItemType<Dropper>());
    }
}
