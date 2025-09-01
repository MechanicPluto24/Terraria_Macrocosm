using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors
{
    public readonly struct ConnectorData : TagSerializable
    {
        public ConnectorType Type { get; }

        public bool Any => Type != ConnectorType.None;
        public bool ConveyorBase => Type == ConnectorType.Conveyor;
        public bool ConveyorInlet => Type == ConnectorType.ConveyorInlet;
        public bool ConveyorOutlet => Type == ConnectorType.ConveyorOutlet;
        public bool AnyConveyor => ConveyorBase || ConveyorInlet || ConveyorOutlet;

        public ConnectorData(ConnectorType type)
        {
            Type = type;
        }

        public TagCompound SerializeData()
        {
            return new()
            {
                [nameof(Type)] = (byte)Type
            };
        }

        public static readonly Func<TagCompound, ConnectorData> DESERIALIZER = DeserializeData;
        public static ConnectorData DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(Type)))
                return new((ConnectorType)tag.GetByte(nameof(Type)));

            return default;
        }
    }
}
