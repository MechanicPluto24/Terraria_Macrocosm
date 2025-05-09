using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using ModLiquidLib.ModLoader;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global
{
    public class MacrocosmGlobalLiquid : GlobalLiquid
    {
        public override void Load()
        {
            On_Liquid.tilesIgnoreWater += On_Liquid_tilesIgnoreWater;
        }

        public override void Unload()
        {
            On_Liquid.tilesIgnoreWater -= On_Liquid_tilesIgnoreWater;
        }

        private static List<int> tilesIgnoreWater;
        private void On_Liquid_tilesIgnoreWater(On_Liquid.orig_tilesIgnoreWater orig, bool ignoreSolids)
        {
            if (tilesIgnoreWater is null)
            {
                tilesIgnoreWater = new();
                for (int type = 0; type < TileLoader.TileCount; type++)
                    if (TileSets.AllowLiquids[type] && Main.tileSolid[type])
                        tilesIgnoreWater.Add(type);
            }

        foreach (int type in tilesIgnoreWater)
            Main.tileSolid[type] = !ignoreSolids;

        orig(ignoreSolids);
    }

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
}
