using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

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

        /// <summary> 
        /// Hook for sliding doors (doors that are always 1x3), copied from WorldGen.OpenDoor, but without 
        /// adjacent solid block checks and all the extra logic needed for an opened 2x3 door.
        /// </summary>
        private bool On_WorldGen_OpenDoor(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            if (TileLoader.GetTile(Main.tile[i, j].TileType) is IClosedSlidingDoor)
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

                if (tile.TileFrameX >= 54)
                {
                    int frameX = tile.TileFrameX / 54;
                    offsetFromOrigin += 36 * frameX;
                    targetFrameX = (short)(targetFrameX + (short)(72 * frameX));
                }

                tilePosX = i;
                tilePosY = j - frameY / 18;

                TileColorCache cache = Main.tile[tilePosX, tilePosY].BlockColorAndCoating();
                TileColorCache cache2 = Main.tile[tilePosX, tilePosY + 1].BlockColorAndCoating();
                TileColorCache cache3 = Main.tile[tilePosX, tilePosY + 2].BlockColorAndCoating();

                if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running)
                {
                    Wiring.SkipWire(tilePosX, tilePosY);
                    Wiring.SkipWire(tilePosX, tilePosY + 1);
                    Wiring.SkipWire(tilePosX, tilePosY + 2);
                }

                targetFrameY = (short)(offsetFromOrigin % 36 * 54);
                SoundEngine.PlaySound(SoundID.DoorOpen, new(i * 16, j * 16));

                ushort openDoorID = (ushort)TileLoader.OpenDoorID(Main.tile[i, j]);

                tile = Main.tile[tilePosX, tilePosY];
                tile.HasTile = true;
                tile.TileType = openDoorID;
                tile.TileFrameY = targetFrameY;
                tile.TileFrameX = targetFrameX;
                tile.UseBlockColors(cache);

                tile = Main.tile[tilePosX, tilePosY + 1];
                tile.HasTile = true;
                tile.TileType = openDoorID;
                tile.TileFrameY = (short)(targetFrameY + 18);
                tile.TileFrameX = targetFrameX;
                tile.UseBlockColors(cache2);

                tile = Main.tile[tilePosX, tilePosY + 2];
                tile.HasTile = true;
                tile.TileType = openDoorID;
                tile.TileFrameY = (short)(targetFrameY + 36);
                tile.TileFrameX = targetFrameX;
                tile.UseBlockColors(cache3);

                return true;
            }

            return orig(i,j,direction);
        }
    }
}
