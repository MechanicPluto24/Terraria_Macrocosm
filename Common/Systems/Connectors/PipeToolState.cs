using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Macrocosm.Common.Systems.Connectors;

public enum PipeToolComponent : byte { Pipes, Inlet, Outlet, Hopper, Dropper }

public struct PipeToolState
{
    public byte Colors;
    public PipeToolComponent Component;
    public bool Cutting;
    public static PipeToolState Default => new() { Colors = 1 };
    public readonly bool IsValid => Colors < 16 && Component <= PipeToolComponent.Dropper;

    public static IEnumerable<Point> Path(Point start, Point end, bool verticalFirst)
    {
        Point p = start;
        yield return p;
        while (p != end)
        {
            if (verticalFirst ? p.Y != end.Y : p.X == end.X)
                p.Y += Math.Sign(end.Y - p.Y);
            else
                p.X += Math.Sign(end.X - p.X);
            yield return p;
        }
    }
}
