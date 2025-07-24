using System;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Macrocosm.Common.Utils;

public static partial class Utility
{
    public static TileObjectData DefaultToPainting(this TileObjectData data, int width, int height, Point16? originOverride = null)
    {
        data.UsesCustomCanPlace = true;

        data.CoordinateWidth = 16;
        data.CoordinatePadding = 2;

        data.Width = width;
        data.Height = height;
        data.CoordinateHeights = new int[height];
        Array.Fill(data.CoordinateHeights, 16);

        data.AnchorWall = true;
        data.LavaDeath = true;

        data.Origin = originOverride ?? (width, height) switch
        {
            (2, 3) => new(0, 1),
            (3, 2) => new(1, 0),
            (3, 3) => new(1, 1),
            (6, 4) => new(2, 2),
            _ => new Point16((width - 1) / 2, (height - 1) / 2) // default of non-vanilla painting sizes
        };

        return data;
    }
}
