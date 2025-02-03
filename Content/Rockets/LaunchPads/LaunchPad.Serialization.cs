using Macrocosm.Common.Storage;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public partial class LaunchPad : TagSerializable
    {
        public LaunchPad Clone() => DeserializeData(SerializeData());

        public static readonly Func<TagCompound, LaunchPad> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Active)] = Active,
                [nameof(StartTile)] = StartTile,
                [nameof(EndTile)] = EndTile,
                [nameof(RocketID)] = RocketID,
                [nameof(internalRocket)] = internalRocket,
                [nameof(Inventory)] = Inventory
            };

            return tag;
        }

        public static LaunchPad DeserializeData(TagCompound tag)
        {
            Point16 startTile = tag.TryGet(nameof(StartTile), out Point16 start) ? start : default;
            Point16 endTile = tag.TryGet(nameof(EndTile), out Point16 end) ? end : default;

            LaunchPad launchPad = new(startTile, endTile)
            {
                Active = tag.ContainsKey(nameof(Active)),
                RocketID = tag.TryGet(nameof(RocketID), out int rocketID) ? rocketID : -1,
                internalRocket = tag.TryGet(nameof(internalRocket), out Rocket inRocket) ? inRocket : new()
            };

            if (tag.ContainsKey(nameof(Inventory)))
            {
                launchPad.Inventory = tag.Get<Inventory>(nameof(Inventory));
                launchPad.Inventory.Size = launchPad.CountRequiredAssemblyItemSlots(out launchPad._moduleSlotRanges);
                launchPad.ReserveAssemblySlots();
            }

            return launchPad;
        }
    }
}
