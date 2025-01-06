using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using SubworldLibrary;
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
        }

        public void Unload()
        {
            On_Liquid.Update -= On_Liquid_Update;
        }

        private void On_Liquid_Update(On_Liquid.orig_Update orig, Liquid self)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                int[] evaporatingLiquidTypes = MacrocosmSubworld.Current.EvaporatingLiquidTypes;
                foreach (int liquidType in evaporatingLiquidTypes)
                {
                    Tile tile = Main.tile[self.x, self.y];
                    if (tile.LiquidType == liquidType && tile.LiquidAmount > 0 && !RoomOxygenSystem.IsRoomPressurized(self.x, self.y))
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
