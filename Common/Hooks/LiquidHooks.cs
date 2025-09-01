using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class LiquidHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Liquid.Update += On_Liquid_Update;
            On_Liquid.tilesIgnoreWater += On_Liquid_tilesIgnoreWater;
        }

        public void Unload()
        {
            On_Liquid.Update -= On_Liquid_Update;
            On_Liquid.tilesIgnoreWater -= On_Liquid_tilesIgnoreWater;
        }

        private static List<int> tilesIgnoreWater;
        private void On_Liquid_tilesIgnoreWater(On_Liquid.orig_tilesIgnoreWater orig, bool ignoreSolids)
        {
            if(tilesIgnoreWater is null)
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

        private void On_Liquid_Update(On_Liquid.orig_Update orig, Liquid self)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                int[] evaporatingLiquidTypes = MacrocosmSubworld.Current.EvaporatingLiquidTypes;
                foreach (int liquidType in evaporatingLiquidTypes)
                {
                    Tile tile = Main.tile[self.x, self.y];
                    if (tile.LiquidType == liquidType && tile.LiquidAmount > 0 && !RoomOxygenSystem.CheckRoomOxygen(self.x, self.y))
                    {
                        byte amount = 2;
                        if (tile.LiquidAmount < amount)
                            amount = tile.LiquidAmount;
                        tile.LiquidAmount -= amount;
                    }
                }
            }

            orig(self);
        }
    }
}
