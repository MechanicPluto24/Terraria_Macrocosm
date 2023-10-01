using System.Collections.Generic;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class PickPowerHook : ILoadable
    {

        private static Dictionary<int, (int minPick, int mineResist, bool onlyInMacrocosm)> vanillaPickPowerByType;

        public void Load(Mod mod)
        {
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;

            vanillaPickPowerByType = new()
            {
                { TileID.LunarOre, (minPick: 210, mineResist: 5, onlyInMacrocosm: false) }
            };
        }

        public void Unload()
        {
            On_Player.GetPickaxeDamage -= On_Player_GetPickaxeDamage;

            vanillaPickPowerByType = null;
        }

        // TML: this could be reworked in a tML GlobalTile "ModifyPickaxeDamage" hook
        private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            int result = orig(self, x, y, pickPower, hitBufferIndex, tileTarget);

            if (vanillaPickPowerByType.ContainsKey(tileTarget.TileType))
            {
                var (minPick, mineResist, onlyInMacrocosm) = vanillaPickPowerByType[tileTarget.TileType];

                if (onlyInMacrocosm && !SubworldSystem.AnyActive<Macrocosm>())
                    return result;

                result = pickPower / mineResist;

                if (pickPower < minPick)
                    result = 0;
            }

            return result;
        }

    }
}
