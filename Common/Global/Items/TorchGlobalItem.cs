using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class TorchGlobalItem : GlobalItem
    {
        public static bool HasFlame(Item item) => flameItems.Contains(item.type);

        private static readonly List<int> flameItems = new();

        public override void SetStaticDefaults()
        {
            // Add all vanilla items that have a flame to the list
            for (int type = 0; type < ItemID.Count; type++)
            {
                Item item = ContentSamples.ItemsByType[type];

                // This excludes non-torch items like the NightGlow
                if (item.flame && ItemID.Sets.Torches[item.type])
                    flameItems.Add(type);
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            // Whenever holding an item that initially had a flame,
            // disable it if a subworld, otherwise set it back 
            if (HasFlame(item) && ItemID.Sets.Torches[item.type])
                item.flame = !SubworldSystem.AnyActive<Macrocosm>();
        }
    }
}