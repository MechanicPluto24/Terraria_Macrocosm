using Macrocosm.Common.Systems.Power;
using System;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Macrocosm.Common.Utils
{
    // Common TileObjectData configurations. Feel free overwrite the defaults after calling the method
    public static partial class Utility
    {
        public static TileObjectData DefaultToPainting(this TileObjectData data, int width, int height)
        {
            data.UsesCustomCanPlace = true;

            // Size
            data.Width = width;
            data.Height = height;
            data.CoordinateHeights = new int[height];
            Array.Fill(data.CoordinateHeights, 16);

            // Common sheet defaults
            data.CoordinateWidth = 16;
            data.CoordinatePadding = 2;

            data.AnchorWall = true;
            data.LavaDeath = true;

            data.Origin = (width, height) switch
            {
                (2, 3) => new(0, 1),
                (3, 2) => new(1, 0),
                (3, 3) => new(1, 1),
                (6, 4) => new(2, 2),
                _ => new Point16((width - 1) / 2, (height - 1) / 2) // default of non-vanilla painting sizes
            };

            return data;
        }

        public static TileObjectData DefaultToMachine(this TileObjectData data, MachineTile machineTile)
        {
            MachineTE machineTE = machineTile.MachineTE;
            data.UsesCustomCanPlace = true;

            // Size
            data.Width = machineTile.Width;
            data.Height = machineTile.Height;
            data.CoordinateHeights = new int[machineTile.Height];
            Array.Fill(data.CoordinateHeights, 16);

            // Common sheet defaults
            data.CoordinateWidth = 16;
            data.CoordinatePadding = 2;

            data.Origin = new Point16(0, machineTile.Height - 1);

            data.HookPostPlaceMyPlayer = new PlacementHook(machineTE.Hook_AfterPlacement, -1, 0, false);
            if (machineTE.InventorySize > 0) 
            {
                data.AnchorInvalidTiles =
                [
                    TileID.MagicalIceBlock,
                    TileID.Boulder,
                    TileID.BouncyBoulder,
                    TileID.LifeCrystalBoulder,
                    TileID.RollingCactus
                ];
            }

            return data;
        }
    }
}
