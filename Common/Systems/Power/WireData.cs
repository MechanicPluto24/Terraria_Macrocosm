using Terraria.ModLoader.IO;
using System;

namespace Macrocosm.Common.Systems.Power
{
    public readonly struct WireData : TagSerializable
    {
        private readonly byte data;

        public bool AnyWire => data != 0;
        public bool CopperWire => (data & 0x01) != 0;

        public static implicit operator WireData(byte value) => new(value);
        public static implicit operator byte(WireData powerWireData) => powerWireData.data;

        // Expand this if needed
        private const int WireTypeMask = 0x01;

        public WireData() { }
        public WireData(WireType type)
        {
            data = type switch
            {
                WireType.None   => new WireData((byte)(data & ~WireTypeMask)),
                WireType.Copper => new WireData((byte)((data & ~WireTypeMask) | 0x01)),
                _ => new(),
            };
        }

        public WireData(byte data)
        {
            this.data = data;
        }

        public TagCompound SerializeData()
        {
            TagCompound tag = new();
            tag[nameof(data)] = data;
            return tag;
        }

        public static readonly Func<TagCompound, WireData> DESERIALIZER = DeserializeData;
        public static WireData DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(data)))
                return new(tag.GetByte(nameof(data)));

            return default;
        }
    }
}
