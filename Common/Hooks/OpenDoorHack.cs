using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public interface IClosedSlidingDoor { }

    public class OpenDoorHack : ILoadable
    {
        public void Load(Mod mod)
        {
            On_WorldGen.OpenDoor += On_WorldGen_OpenDoor;
        }

        public void Unload()
        {
            On_WorldGen.OpenDoor -= On_WorldGen_OpenDoor;
        }

        private bool On_WorldGen_OpenDoor(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            if (TileLoader.GetTile(Main.tile[i, j].TileType) is IClosedSlidingDoor)
                 direction = Math.Abs(direction);

            return orig(i,j,direction);
        }

    }
}
