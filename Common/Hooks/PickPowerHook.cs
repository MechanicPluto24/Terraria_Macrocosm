using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class PickPowerHook : ILoadable
    {

        private static Dictionary<int, (int minPick, int mineResist, bool onlyInMacrocosm)> modifiedPickPowerByType;

        public void Load(Mod mod)
        {
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;

            modifiedPickPowerByType = new()
            {
                { TileID.LunarOre, (minPick: 210, mineResist: 5, onlyInMacrocosm: false) }
            };
        }

        public void Unload()
        {
            On_Player.GetPickaxeDamage -= On_Player_GetPickaxeDamage;

            modifiedPickPowerByType = null;
        }

        // TML: this could be reworked into a tML GlobalTile "ModifyPickaxeDamage" hook
        private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            int result = orig(self, x, y, pickPower, hitBufferIndex, tileTarget);

            if (modifiedPickPowerByType.ContainsKey(tileTarget.TileType))
            {
                var (minPick, mineResist, onlyInMacrocosm) = modifiedPickPowerByType[tileTarget.TileType];

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
