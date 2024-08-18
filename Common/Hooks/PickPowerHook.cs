using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    // TML: this could be reworked into a tML GlobalTile "ModifyPickaxeDamage" hook
    public class PickPowerHook : ILoadable
    {
        private static Dictionary<ushort, (int minPick, int mineResist, bool onlyInMacrocosm)> modifiedPickPowerByType;

        public void Load(Mod mod)
        {
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;
            modifiedPickPowerByType = new();
        }

        public void Unload()
        {
            On_Player.GetPickaxeDamage -= On_Player_GetPickaxeDamage;
            modifiedPickPowerByType = null;
        }

        public static void RegisterPickPowerModification(ushort tileType, int minPick, int mineResist, bool onlyInMacrocosm)
        {
            modifiedPickPowerByType.Add(tileType, (minPick, mineResist, onlyInMacrocosm));
        }

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
