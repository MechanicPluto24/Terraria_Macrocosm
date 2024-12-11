using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Machines;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class RoomOxygenSystem : ModSystem
    {
        public static bool IsRoomPressurized(int x, int y)
        {
            if (!TryGetRoomBounds(x, y, out int startX, out int endX, out int startY, out int endY))
                return false;

            for (int tileX = startX; tileX <= endX; tileX++)
            {
                for (int tileY = startY; tileY <= endY; tileY++)
                {
                    if (Utility.TryGetTileEntityAs<MachineTE>(tileX, tileY, out var te) && te is IOxygenSource oxygenSource && oxygenSource.IsProvidingOxygen)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsRoomPressurized(Point tileCoords) => IsRoomPressurized(tileCoords.X, tileCoords.Y);    
        public static bool IsRoomPressurized(Vector2 worldPosition) => IsRoomPressurized(worldPosition.ToTileCoordinates());    
        public static bool IsRoomPressurized(Entity entity) => IsRoomPressurized(entity.Center);    

        private static bool TryGetRoomBounds(int x, int y, out int startX, out int endX, out int startY, out int endY)
        {
            if (!WorldGen.StartRoomCheck(x, y))
            {
                startX = endX = startY = endY = 0;
                return false;
            }

            startX = WorldGen.roomX1;
            endX = WorldGen.roomX2;
            startY = WorldGen.roomY1;
            endY = WorldGen.roomY2;

            return true;
        }
    }
}
