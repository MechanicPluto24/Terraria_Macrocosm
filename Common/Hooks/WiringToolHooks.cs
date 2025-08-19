using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace Macrocosm.Common.Hooks;

public class WiringToolHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_Player.ItemCheck_UseWiringTools += On_Player_ItemCheck_UseWiringTools;
    }

    public void Unload()
    {
        On_Player.ItemCheck_UseWiringTools -= On_Player_ItemCheck_UseWiringTools;
    }

    // Can't use UseItem here as it doesn't fail early (calls every hook & can't skip vanilla logic)
    private void On_Player_ItemCheck_UseWiringTools(On_Player.orig_ItemCheck_UseWiringTools orig, Player player, Item item)
    {
        if (player.whoAmI == Main.myPlayer && player.ItemInTileReach(item) && player.CanDoWireStuffHere(Player.tileTargetX, Player.tileTargetY) && player.itemAnimation > 0 && player.ItemTimeIsZero && player.controlUseItem)
        {
            Point pos = player.TargetCoords();
            Tile tile = player.TargetTile();
            if (item.type == ItemID.WireCutter)
            {
                // Run default Wire Cutter logic
                orig(player, item);

                // If the Wire Cutter didn't do anything, try removing Pipes, Inlets, Outlets
                //if (!targetTile.HasWire() && !targetTile.HasActuator)
                if (player.ItemTimeIsZero && ConveyorSystem.Remove(pos))
                    player.ApplyItemTime(item);

                return;
            }
            else if (item.type == ItemID.MulticolorWrench)
            {
                if((WiresUI.Settings.ToolMode & WiresUI.Settings.MultiToolMode.Cutter) > 0)
                {
                    // TODO: wire cutting
                }
                else if (PickPipe(player, tile.HasWire(), out _))
                {
                    bool any = false;
                    any |= TryPlace(player, ConveyorPipeType.RedPipe, sync: false, WiresUI.Settings.MultiToolMode.Red);
                    any |= TryPlace(player, ConveyorPipeType.GreenPipe, sync: false, WiresUI.Settings.MultiToolMode.Green);
                    any |= TryPlace(player, ConveyorPipeType.BluePipe, sync: false, WiresUI.Settings.MultiToolMode.Blue);
                    any |= TryPlace(player, ConveyorPipeType.YellowPipe, sync: false, WiresUI.Settings.MultiToolMode.Yellow);

                    if (any)
                    {
                        if(Main.netMode == NetmodeID.MultiplayerClient)
                            ConveyorSystem.SyncConveyor(pos.X, pos.Y, dustEffects: true);

                        player.ApplyItemTime(item);
                        return;
                    }
                }
            }
            else if (WrenchToPipe(item.type, out ConveyorPipeType type))
            {
                if (TryPlace(player, type, sync: true))
                {
                    player.ApplyItemTime(item);
                    return;
                }
            }
        }

        orig(player, item);
    }

    private static bool TryPlace(Player player, ConveyorPipeType pipeType, bool sync, WiresUI.Settings.MultiToolMode mode = 0)
    {
        if (mode == 0 || (WiresUI.Settings.ToolMode & mode) != 0)
        {
            if (PickPipe(player, skipWires: true, out int slot))
            {
                Point pos = player.TargetCoords();
                if (ConveyorSystem.PlacePipe(pos, pipeType, sync))
                {
                    player.inventory[slot].DecreaseStack(1, player);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary> Wrench Item ID to <see cref="ConveyorPipeType"/></summary>
    private static bool WrenchToPipe(int itemType, out ConveyorPipeType pipeType)
    {
        switch (itemType)
        {
            case ItemID.Wrench:
                pipeType = ConveyorPipeType.RedPipe;
                return true;

            case ItemID.BlueWrench:
                pipeType = ConveyorPipeType.BluePipe;
                return true;

            case ItemID.GreenWrench:
                pipeType = ConveyorPipeType.GreenPipe;
                return true;

            case ItemID.YellowWrench:
                pipeType = ConveyorPipeType.YellowPipe;
                return true;

            default:
                pipeType = 0;
                return false;
        }
    }

    /// <summary> Used to pick pipes over wires, in ammo priority </summary>
    private static bool PickPipe(Player player, bool skipWires, out int slot)
    {
        slot = -1;
        // Ammo slots first, top to bottom
        for (int i = 55; i <= 58; i++)
        {
            Item item = player.inventory[i];
            if (item.type == ItemID.Wire && !skipWires)
            {
                return false;
            }

            if (item.type == ModContent.ItemType<Conveyor>())
            {
                slot = i;
                return true;
            }
        }

        // Main inventory slots, from top-left
        for (int i = 0; i <= 54; i++)
        {
            Item item = player.inventory[i];
            if (item.type == ItemID.Wire && !skipWires)
            {
                return false;
            }

            if (item.type == ModContent.ItemType<Conveyor>())
            {
                slot = i;
                return true;
            }
        }

        return false;
    }
}
