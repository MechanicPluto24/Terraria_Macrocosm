using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
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

    /// <summary>
    /// Attachments:
    /// <code>
    /// bit0 = present,
    /// bit1 = type(0=Dropper,1=Hopper), 
    /// bits2-3 = rotation(0..3)
    /// </code>
    /// </summary>

    private BitsByte data2;

    public ConveyorData()
    {
        data = 0;
        data2 = 0;
    }

    public ConveyorData(byte packed)
    {
        data = packed;
        data2 = 0;
    }

    public ConveyorData(ushort packed)
    {
        data = (byte)(packed & 0xFF);
        data2 = (byte)(packed >> 8);
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

    public readonly ushort Packed => (ushort)(data | ((ushort)(byte)data2 << 8));

    public bool RedPipe { get => data[0]; set => data[0] = value; }
    public bool GreenPipe { get => data[1]; set => data[1] = value; }
    public bool BluePipe { get => data[2]; set => data[2] = value; }
    public bool YellowPipe { get => data[3]; set => data[3] = value; }
    public bool Outlet { get => AnyPipe && data[4] && !data[5]; set => (data[4], data[5]) = (value && AnyPipe, false); }
    public bool Inlet { get => AnyPipe && data[5] && !data[4]; set => (data[5], data[4]) = (value && AnyPipe, false); }
    public bool Attachment { get => data2[0]; set { data2[0] = value; if (!data2[0]) AttachmentRotation = 0; } }
    public bool AttachmentIsHopper { get => data2[1]; set => data2[1] = value; }
    public byte AttachmentRotation
    {
        get => (byte)(((data2[2] ? 1 : 0) | (data2[3] ? 2 : 0)) & 0b11);
        set
        {
            byte masked = (byte)(value & 0b11);
            data2[2] = (masked & 0b01) != 0;
            data2[3] = (masked & 0b10) != 0;
        }
    }

    public bool Dropper { get => Attachment && !AttachmentIsHopper; set { Attachment = value; AttachmentIsHopper = false; if (!value) AttachmentRotation = 0; } }
    public bool Hopper { get => Attachment && AttachmentIsHopper; set { Attachment = value; AttachmentIsHopper = true; if (!value) AttachmentRotation = 0; } }

    public bool AnyPipe => RedPipe || GreenPipe || BluePipe || YellowPipe;
    public int PipeCount => (RedPipe ? 1 : 0) + (GreenPipe ? 1 : 0) + (BluePipe ? 1 : 0) + (YellowPipe ? 1 : 0);

    public bool IsValidForConveyorNode(ConveyorPipeType? pipe = null)
    {
        bool hasPipe = pipe.HasValue ? HasPipe(pipe.Value) : AnyPipe;
        return (hasPipe && (Inlet || Outlet)) || Attachment;
    }

    public bool HasPipe(ConveyorPipeType type)
    {
        return type switch
        {
            ConveyorPipeType.RedPipe => RedPipe,
            ConveyorPipeType.GreenPipe => GreenPipe,
            ConveyorPipeType.BluePipe => BluePipe,
            ConveyorPipeType.YellowPipe => YellowPipe,
            _ => false,
        };
    }

    public void SetPipe(ConveyorPipeType type)
    {
        switch (type)
        {
            case ConveyorPipeType.RedPipe: RedPipe = true; break;
            case ConveyorPipeType.GreenPipe: GreenPipe = true; break;
            case ConveyorPipeType.BluePipe: BluePipe = true; break;
            case ConveyorPipeType.YellowPipe: YellowPipe = true; break;
            default: break;
        }
    }

    public void ClearPipe(ConveyorPipeType type)
    {
        switch (type)
        {
            case ConveyorPipeType.RedPipe: RedPipe = false; break;
            case ConveyorPipeType.GreenPipe: GreenPipe = false; break;
            case ConveyorPipeType.BluePipe: BluePipe = false; break;
            case ConveyorPipeType.YellowPipe: YellowPipe = false; break;
            default: break;
        }

    }

    public void ClearAll()
    {
        data = 0;
        data2 = 0;
    }
}
