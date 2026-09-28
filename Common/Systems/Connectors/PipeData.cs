using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PipeData : ITileData
{
    /// <summary>
    /// <code>
    ///  bits 0-3: pipe mask R/G/B/Y
    ///  bits 4-5: endpoint mode, 0=None, 1=Inlet, 2=Outlet, 3=reserved
    ///  bit 6: attachment present
    ///  bit 7: attachment type, 0=Dropper, 1=Hopper
    ///  bits 8-9: attachment rotation, 0..3
    ///  bits 10-15: reserved for future use
    /// </code>
    /// </summary>
    private ushort data;

    private const ushort RedPipeBit = 1 << 0;
    private const ushort GreenPipeBit = 1 << 1;
    private const ushort BluePipeBit = 1 << 2;
    private const ushort YellowPipeBit = 1 << 3;
    private const ushort PipeMask = RedPipeBit | GreenPipeBit | BluePipeBit | YellowPipeBit;

    private const int EndpointShift = 4;
    private const ushort EndpointMask = 0b11 << EndpointShift;

    private const ushort AttachmentBit = 1 << 6;
    private const ushort AttachmentHopperBit = 1 << 7;

    private const int AttachmentRotationShift = 8;
    private const ushort AttachmentRotationMask = 0b11 << AttachmentRotationShift;

    private enum PipeEndpoint : byte
    {
        None = 0,
        Inlet = 1,
        Outlet = 2,
    }

    public PipeData()
    {
        data = 0;
    }

    public PipeData(byte packed)
    {
        data = packed;
    }

    public PipeData(ushort packed)
    {
        data = packed;
    }

    public PipeData(bool red = false, bool green = false, bool blue = false, bool yellow = false, bool outlet = false, bool inlet = false) : this()
    {
        RedPipe = red;
        GreenPipe = green;
        BluePipe = blue;
        YellowPipe = yellow;
        Outlet = outlet;
        Inlet = inlet;
    }

    public readonly ushort Packed => data;

    public bool RedPipe { readonly get => HasFlag(RedPipeBit); set => SetFlag(RedPipeBit, value); }
    public bool GreenPipe { readonly get => HasFlag(GreenPipeBit); set => SetFlag(GreenPipeBit, value); }
    public bool BluePipe { readonly get => HasFlag(BluePipeBit); set => SetFlag(BluePipeBit, value); }
    public bool YellowPipe { readonly get => HasFlag(YellowPipeBit); set => SetFlag(YellowPipeBit, value); }
    public bool Outlet { readonly get => AnyPipe && Endpoint == PipeEndpoint.Outlet; set => Endpoint = value && AnyPipe ? PipeEndpoint.Outlet : PipeEndpoint.None; }
    public bool Inlet { readonly get => AnyPipe && Endpoint == PipeEndpoint.Inlet; set => Endpoint = value && AnyPipe ? PipeEndpoint.Inlet : PipeEndpoint.None; }
    public bool Attachment { readonly get => HasFlag(AttachmentBit); set { SetFlag(AttachmentBit, value); if (!value) AttachmentRotation = 0; } }
    public bool AttachmentIsHopper { readonly get => HasFlag(AttachmentHopperBit); set => SetFlag(AttachmentHopperBit, value); }
    public byte AttachmentRotation
    {
        readonly get => (byte)((data & AttachmentRotationMask) >> AttachmentRotationShift);
        set
        {
            byte masked = (byte)(value & 0b11);
            data = (ushort)((data & ~AttachmentRotationMask) | (masked << AttachmentRotationShift));
        }
    }

    public bool Dropper { readonly get => Attachment && !AttachmentIsHopper; set { Attachment = value; AttachmentIsHopper = false; if (!value) AttachmentRotation = 0; } }
    public bool Hopper { readonly get => Attachment && AttachmentIsHopper; set { Attachment = value; AttachmentIsHopper = value; if (!value) AttachmentRotation = 0; } }

    public readonly bool AnyPipe => (data & PipeMask) != 0;
    public readonly int PipeCount => (RedPipe ? 1 : 0) + (GreenPipe ? 1 : 0) + (BluePipe ? 1 : 0) + (YellowPipe ? 1 : 0);

    private PipeEndpoint Endpoint
    {
        readonly get => (PipeEndpoint)((data & EndpointMask) >> EndpointShift);
        set => data = (ushort)((data & ~EndpointMask) | ((ushort)value << EndpointShift));
    }

    public readonly bool IsValidForPipeNode(PipeType? pipe = null)
    {
        bool hasPipe = pipe.HasValue ? HasPipe(pipe.Value) : AnyPipe;
        return (hasPipe && (Inlet || Outlet)) || Attachment;
    }

    public readonly bool HasPipe(PipeType type)
    {
        return type switch
        {
            PipeType.RedPipe => RedPipe,
            PipeType.GreenPipe => GreenPipe,
            PipeType.BluePipe => BluePipe,
            PipeType.YellowPipe => YellowPipe,
            _ => false,
        };
    }

    public void SetPipe(PipeType type)
    {
        switch (type)
        {
            case PipeType.RedPipe: RedPipe = true; break;
            case PipeType.GreenPipe: GreenPipe = true; break;
            case PipeType.BluePipe: BluePipe = true; break;
            case PipeType.YellowPipe: YellowPipe = true; break;
            default: break;
        }
    }

    public void ClearPipe(PipeType type)
    {
        switch (type)
        {
            case PipeType.RedPipe: RedPipe = false; break;
            case PipeType.GreenPipe: GreenPipe = false; break;
            case PipeType.BluePipe: BluePipe = false; break;
            case PipeType.YellowPipe: YellowPipe = false; break;
            default: break;
        }

        if (!AnyPipe)
            Endpoint = PipeEndpoint.None;
    }

    public void ClearAll()
    {
        data = 0;
    }

    private readonly bool HasFlag(ushort flag)
    {
        return (data & flag) != 0;
    }

    private void SetFlag(ushort flag, bool value)
    {
        if (value)
            data |= flag;
        else
            data = (ushort)(data & ~flag);
    }
}
