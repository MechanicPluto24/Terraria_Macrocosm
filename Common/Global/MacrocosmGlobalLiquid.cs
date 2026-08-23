using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using ModLiquidLib.ModLoader;
using SubworldLibrary;
using Terraria;

namespace Macrocosm.Common.Global;

public class MacrocosmGlobalLiquid : GlobalLiquid
{
    public override bool UpdateLiquid(int i, int j, int type, Liquid liquid)
    {
        if (SubworldSystem.AnyActive<Macrocosm>())
        {
            int[] evaporatingLiquidTypes = MacrocosmSubworld.Current.EvaporatingLiquidTypes;
            foreach (int liquidType in evaporatingLiquidTypes)
            {
                Tile tile = Main.tile[i, j];
                if (type == liquidType && tile.LiquidAmount > 0 && !RoomOxygenSystem.CheckRoomOxygen(i, j))
                {
                    byte amount = 2;
                    if (tile.LiquidAmount < amount)
                        amount = tile.LiquidAmount;
                    tile.LiquidAmount -= amount;
                }
            }
        }
        return true;
    }
}
