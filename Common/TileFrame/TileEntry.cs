using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.ObjectData;

namespace Macrocosm.Common.TileFrame
{
    public readonly struct TileEntry
    {
        public ushort Type { get; init; }

        public int GetStyle() => WorldGen.genRand.Next(styles);
        private readonly Range styles;

        public bool IsValid => isValid; 

        private readonly bool isValid;
        public TileObjectData GetData(int style, int alternate = 0) => TileObjectData.GetTileData(Type, style, alternate);

        public TileEntry(int type, int style = 0)
        {
            Type = (ushort)type;
            styles = new(style, style);
            isValid = true;
        }

        public TileEntry(int type, Range styles)
        {
            Type = (ushort)type;
            this.styles = styles;
            isValid = true;
        }
    }
}
