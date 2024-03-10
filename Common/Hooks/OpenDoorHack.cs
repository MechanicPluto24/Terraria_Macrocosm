using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

namespace Macrocosm.Common.Hooks
{
    public interface IClosedSlidingDoor 
    {
        public int DoorHeight => 3;
        public int StyleCount => 1;

    }

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

        /// <summary> 
        /// Hook for sliding doors (doors that are always 1x3), copied from WorldGen.OpenDoor, but without 
        /// adjacent solid block checks and all the extra logic needed for an opened 2x3 door.
        /// </summary>
        private bool On_WorldGen_OpenDoor(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            if (TileLoader.GetTile(Main.tile[i, j].TileType) is IClosedSlidingDoor door)
            {
                Tile tile = Main.tile[i, j];
                int tilePosX, tilePosY;
                short targetFrameY, targetFrameX = 0;
                int frameY = tile.TileFrameY;
                int offsetFromOrigin = 0;

                if (TileLoader.OpenDoorID(tile) < 0)
                    return false;

                if (WorldGen.IsLockedDoor(tile))
                    return false;

                while (frameY >= 54)
                {
                    frameY -= 54;
                    offsetFromOrigin++;
                }

                if (tile.TileFrameX >= 18 * door.StyleCount)
                {
                    int frameX = tile.TileFrameX / (18 * door.StyleCount);
                    offsetFromOrigin += 36 * frameX; // Not sure of this
                    targetFrameX = (short)(targetFrameX + (short)((18 + 18 * door.StyleCount) * frameX));
                }

                tilePosX = i;
                tilePosY = j - frameY / 18;

                targetFrameY = (short)((offsetFromOrigin % 36) * (18 * door.DoorHeight));
                SoundEngine.PlaySound(SoundID.DoorOpen, new(i * 16, j * 16));

                ushort openDoorID = (ushort)TileLoader.OpenDoorID(Main.tile[i, j]);

                for(int y = 0; y < door.DoorHeight; y++)
                {
                    TileColorCache cache = Main.tile[tilePosX, tilePosY + y].BlockColorAndCoating();

                    if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running)
                         Wiring.SkipWire(tilePosX, tilePosY + y);

                    tile = Main.tile[tilePosX, tilePosY + y];
                    tile.HasTile = true;
                    tile.TileType = openDoorID;
                    tile.TileFrameY = (short)(targetFrameY + y * 18);
                    tile.TileFrameX = targetFrameX;
                    tile.UseBlockColors(cache);
                }

                return true;
            }

            return orig(i,j,direction);
        }
    }
}
