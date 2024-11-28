using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors
{
    public readonly struct ConnectorData : TagSerializable
    {
        private readonly byte data;

        public bool AnyWire => data != 0;
        public bool Conveyor => (data & 0x01) != 0;

        public static implicit operator ConnectorData(byte value) => new(value);
        public static implicit operator byte(ConnectorData powerWireData) => powerWireData.data;

        // Expand this if needed
        private const int TypeMask = 0x01;

        public ConnectorData() { }
        public ConnectorData(ConnectorType type)
        {
            data = type switch
            {
                ConnectorType.None => new ConnectorData((byte)(data & ~TypeMask)),
                ConnectorType.Conveyor => new ConnectorData((byte)(data & ~TypeMask | 0x01)),
                _ => new(),
            };
        }

        public ConnectorData(byte data)
        {
            this.data = data;
        }

        public TagCompound SerializeData()
        {
            TagCompound tag = new();
            tag[nameof(data)] = data;
            return tag;
        }

        public static readonly Func<TagCompound, ConnectorData> DESERIALIZER = DeserializeData;
        public static ConnectorData DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(data)))
                return new(tag.GetByte(nameof(data)));

            return default;
        }
    }
}
