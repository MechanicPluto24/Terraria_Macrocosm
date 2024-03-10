using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent;
using MonoMod.Cil;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;

namespace Macrocosm.Common.Hooks
{
    public interface IDoorTile
    {
        /// <summary> Door height in tiles. Should be >= 1 </summary>
        public int Height { get; }

        /// <summary> 
        /// Door width in tiles.
        /// For the current implementation, it should be 1 for closed doors; 1 or 2 for open doors. 
        /// </summary>
        public int Width { get; }

        /// <summary> Whether this tile is the closed state of the door </summary>
        public bool IsClosed { get; }

        /// <summary> Whether this door is locked <b>(TODO)</b></summary>
        public bool IsLocked => false;

        /// <summary>
        /// Number of styles for this door. 
        /// For closed doors, it means the number of alternate styles.
        /// For open doors, it should be 1 for sliding doors, or 2 for hinged doors.
        /// </summary>
        public int StyleCount { get; }

        /// <summary> Number of Y frames used solely for animation </summary>
        public int AnimationFrames => 0;

        /// <summary> The door activate sound. Defaults to <see cref="SoundID.DoorOpen"/> or <see cref="SoundID.DoorClosed"/>, depending on <see cref="IsClosed"/>. </summary>
        public SoundStyle? ActivateSound => null;

        public Rectangle ModifyAutoDoorPlayerCollisionRectangle(Point tileCoords, Rectangle original)
        {
            return original;
        }
    }

    public class DoorTileHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_WorldGen.OpenDoor += On_WorldGen_OpenDoor;
            On_WorldGen.CloseDoor += On_WorldGen_CloseDoor;
            IL_DoorOpeningHelper.CommonDoorOpeningInfoProvider.TryOpenDoor += ModifyAutoDoorPlayerCollisionRectangleILHook;
            IL_DoorOpeningHelper.CommonDoorOpeningInfoProvider.TryCloseDoor += ModifyAutoDoorPlayerCollisionRectangleILHook;
        }


        public void Unload()
        {
            On_WorldGen.OpenDoor -= On_WorldGen_OpenDoor;
            On_WorldGen.CloseDoor -= On_WorldGen_CloseDoor;
            IL_DoorOpeningHelper.CommonDoorOpeningInfoProvider.TryOpenDoor -= ModifyAutoDoorPlayerCollisionRectangleILHook;
            IL_DoorOpeningHelper.CommonDoorOpeningInfoProvider.TryCloseDoor -= ModifyAutoDoorPlayerCollisionRectangleILHook;
        }

        /// <summary> Hook for opening custom modded doors. </summary>
        private bool On_WorldGen_OpenDoor(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            if (TileLoader.GetTile(Main.tile[i, j].TileType) is IDoorTile door && door.IsClosed)
            {
                Tile tile = Main.tile[i, j];
                int tilePosX, tilePosY;
                short targetFrameY, targetFrameX = 0;
                int frameY = tile.TileFrameY;
                int offsetFromOrigin = 0;

                if (TileLoader.OpenDoorID(tile) < 0)
                    return false;

                if (door.IsLocked)
                    return false;

                while (frameY >= 18 * door.Height)
                {
                    frameY -= 18 * door.Height;
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

                targetFrameY = (short)((offsetFromOrigin % 36) * (18 * door.Height));

                if (door.AnimationFrames > 0)
                {
                    Animation.NewTemporaryAnimation(0, tile.TileType, i, j);
                    targetFrameY += (short)(18 * door.Height * door.AnimationFrames);
                }

                SoundEngine.PlaySound(door.ActivateSound ?? SoundID.DoorOpen, new(i * 16, j * 16));

                ushort openDoorID = (ushort)TileLoader.OpenDoorID(Main.tile[i, j]);

                for(int x = 0; x < door.Width; x++)
                {
                    for (int y = 0; y < door.Height; y++)
                    {
                        // NOTE: No x indexing here
                        TileColorCache cache = Main.tile[tilePosX, tilePosY + y].BlockColorAndCoating();

                        if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running)
                            Wiring.SkipWire(tilePosX + x, tilePosY + y);

                        tile = Main.tile[tilePosX + x, tilePosY + y];
                        tile.HasTile = true;
                        tile.TileType = openDoorID;
                        tile.TileFrameY = (short)(targetFrameY + y * 18);
                        tile.TileFrameX = targetFrameX;
                        tile.UseBlockColors(cache);
                    }
                }

                /*
                for (int x = tilePosX - 1; x <= tilePosX + 2; x++)
                    for (int y = tilePosY - 1; y <= tilePosY + 2; y++)
                        WorldGen.TileFrame(x, y);
                */

                return true;
            }

            return orig(i,j,direction);
        }

        /// <summary> Hook for closing custom modded doors. </summary>
        private bool On_WorldGen_CloseDoor(On_WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            if (TileLoader.GetTile(Main.tile[i, j].TileType) is IDoorTile door && !door.IsClosed)
            {
                int direction = 0;
                int tilePosX = i;
                int tilePosY;

                int frameX = Main.tile[i, j].TileFrameX;
                Tile tile = Main.tile[i, j];
                if (TileLoader.CloseDoorID(Main.tile[i, j]) < 0)
                    return false;

                int frameY = tile.TileFrameY;
                int offsetFromOrigin = 0;
                int targetFrameY = frameY;
                int targetFrameX = 0;
                while (frameY >= 18 * door.Height)
                {
                    frameY -= 18 * door.Height;
                    offsetFromOrigin++;
                }

                if (frameX >= 18 * door.StyleCount * door.Width)
                {
                    int huh = 54;
                    offsetFromOrigin += (18 * door.StyleCount) * (frameX / (18 * door.StyleCount * door.Width));
                    targetFrameX += huh * (frameX / (18 * door.StyleCount * door.Width));
                }

                tilePosY = j - frameY / 18;
                switch (frameX % (18 * door.StyleCount * door.Width))
                {
                    case 0:
                        tilePosX = i;
                        direction = 1;
                        break;
                    case 18:
                        tilePosX = i - 1;
                        direction = 1;
                        break;
                    case 36:
                        tilePosX = i + 1;
                        direction = -1;
                        break;
                    case 54:
                        tilePosX = i;
                        direction = -1;
                        break;
                }

                int doorOpenTilePosX = tilePosX;
                if (direction == -1)
                    doorOpenTilePosX = tilePosX - 1;

                if (!forced)
                {
                    for (int k = tilePosY; k < tilePosY + 3; k++)
                    {
                        if (!Collision.EmptyTile(tilePosX, k, ignoreTiles: true))
                            return false;
                    }
                }

                ushort closeDoorID = (ushort)TileLoader.CloseDoorID(tile);

                

                for (int x = doorOpenTilePosX; x < doorOpenTilePosX + door.Width; x++)
                {
                    for (int y = tilePosY; y < tilePosY + door.Height; y++)
                    {
                        tile = Main.tile[x, y];
                        if (x == tilePosX)
                        {
                            tile.TileType = closeDoorID;
                            if (TileLoader.GetTile(closeDoorID) is IDoorTile closedDoor)
                            {
                                tile.TileFrameX = (short)(WorldGen.genRand.Next(closedDoor.StyleCount) * 18 + targetFrameX);
                                if (door.AnimationFrames > 0)
                                {
                                    Animation.NewTemporaryAnimation(0, tile.TileType, i, j);
                                    tile.TileFrameY += (short)(18 * door.Height * door.AnimationFrames);
                                }
                            }
                        }
                        else
                        {
                            tile.HasTile = false;
                        }
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running)
                {
                    for(int y = 0; y < door.Height; y++)
                    {
                        Wiring.SkipWire(tilePosX, tilePosY + y);
                    }
                }

                /*
                for (int x = tilePosX - 1; x <= tilePosX + 1; x++)
                {
                    for (int y = tilePosY - 1; y <= tilePosY + 2; y++)
                    {
                        WorldGen.TileFrame(x, y);
                    }
                }
                */

                SoundEngine.PlaySound(door.ActivateSound ?? SoundID.DoorClosed, new(i * 16, j * 16));
                return true;
            }

            return orig(i,j,forced);
        }

        private void ModifyAutoDoorPlayerCollisionRectangleILHook(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchCall(typeof(Rectangle), ".ctor")))
            {
                Macrocosm.Instance.Logger.Error("Failed to inject ILHook: ModifyDoorOpenCollisionRectangleILHook");
                return;
            }

            // Load local variable 0 (Point tileCoordsForToggling) onto the eval stack.
            c.Emit(OpCodes.Ldloc, 0);

            // Load the adress local variable 2 (Rectangle rectangle) onto the eval stack.
            c.Emit(OpCodes.Ldloca, 2);

            // Hook to modify the rectangle
            c.EmitDelegate(ModifyAutoDoorPlayerCollisionRectangle);
        }

        private void ModifyAutoDoorPlayerCollisionRectangle(Point tileCoords, ref Rectangle original)
        {
            Tile tile = Main.tile[tileCoords];
            if (TileLoader.GetTile(tile.TileType) is IDoorTile door)
                original = door.ModifyAutoDoorPlayerCollisionRectangle(tileCoords, original);
        }

        /*
        private void GetCustomDoorHeight(ILContext il)
        {
            var c = new ILCursor(il);

            // Find where '48' is loaded onto the eval stack.
            if (!c.TryGotoNext(i => i.MatchLdcI4(48)))
                return;

            // Load arg0 (Point tileCoordsForToggling) onto the eval stack.
            c.Emit(OpCodes.Ldarg_0);

            c.EmitDelegate(GetDoorHeightForPlayerOpenDoor);
        }

        private int GetDoorHeightForPlayerOpenDoor(Point tileCoords, int baseValue)
        {
            Tile tile = Main.tile[tileCoords];
            if (TileLoader.GetTile(tile.TileType) is IDoorTile door)
            {
                return door.Height * 16;
            }

            return baseValue;
        }
        */
    }
}
