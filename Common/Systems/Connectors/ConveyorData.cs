using System;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public struct ConveyorData : ITileData
    {       
        /// <summary>
        /// <code>
        ///  -- 00IOYBGR --
        ///  R = whether the tile has a regular ("red") Pipe.
        ///  G = whether the tile has a green Pipe.
        ///  B = whether the tile has a blue Pipe.
        ///  Y = whether the tile has a yellow Pipe.
        ///  O = whether the tile has an Outlet. Only valid if R|G|B|Y.
        ///  I = whether the tile has an Intlet. Only valid if R|G|B|Y.
        ///  IO == 11 is not valid and must be guarded against.
        ///  00 = reserved for future use.
        /// </code> 
        /// </summary>
        private BitsByte data;

        public ConveyorData()
        {
            data = 0;
        }

        public ConveyorData(byte packed)
        {
            data = packed;
        }

        public ConveyorData(bool red = false, bool green = false, bool blue = false, bool yellow = false, bool outlet = false, bool inlet = false) : this()
        {
            RedPipe = red;
            GreenPipe = green;
            BluePipe = blue;
            YellowPipe = yellow;
            Outlet = outlet;
            Inlet = inlet;
        }

        public readonly byte Packed => data;
        public bool AnyPipe => data[0] || data[1] || data[2] || data[3];
        public bool RedPipe { get => data[0]; set => data[0] = value; }
        public bool GreenPipe { get => data[1]; set => data[1] = value; }
        public bool BluePipe { get => data[2]; set => data[2] = value; }
        public bool YellowPipe { get => data[3]; set => data[3] = value; }
        public bool Outlet { get => AnyPipe && data[4] && !data[5]; set => (data[4], data[5]) = (value && AnyPipe, false); }
        public bool Inlet { get => AnyPipe && data[5] && !data[4]; set => (data[5], data[4]) = (value && AnyPipe, false); }

        public void Set(ConveyorType type)
        {
            switch (type)
            {
                case ConveyorType.RedPipe: RedPipe = true; break;
                case ConveyorType.GreenPipe: GreenPipe = true; break;
                case ConveyorType.BluePipe: BluePipe = true; break;
                case ConveyorType.YellowPipe: YellowPipe = true; break;
                default: break;
            }
        }

        public bool HasPipe(ConveyorType type)
        {
            return type switch
            {
                ConveyorType.RedPipe => RedPipe,
                ConveyorType.GreenPipe => GreenPipe,
                ConveyorType.BluePipe => BluePipe,
                ConveyorType.YellowPipe => YellowPipe,
                _ => false,
            };
        }

        public void Clear(ConveyorType type)
        {
            switch (type)
            {
                case ConveyorType.RedPipe: RedPipe = false; break;
                case ConveyorType.GreenPipe: GreenPipe = false; break;
                case ConveyorType.BluePipe: BluePipe = false; break;
                case ConveyorType.YellowPipe: YellowPipe = false; break;
                default: break;
            }
        }

        public void Clear()
        {
            RedPipe = false;
            GreenPipe = false;
            BluePipe = false;
            YellowPipe = false;
            Outlet = false;
            Inlet = false;
        }
    }
}
