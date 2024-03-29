using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Global
{
    public class TorchGlobalItem : GlobalItem
    {
        /// <summary> Returns whether the provided item type is a torch </summary>
        public static bool IsTorch(int itemId) => ItemID.Sets.Torches[itemId];

        /// <summary> Returns whether the provided item instance is a torch </summary>
        public static bool IsTorch(Item item) => IsTorch(item.type);
        public static bool HasFlame(Item item) => flameItems.Contains(item.type);

        private static readonly List<int> flameItems = new();

        public override void Load()
        {
            // add all vanilla items that have a flame to the list
            for (int type = 0; type < ItemID.Count; type++)
            {
                Item item = new(type); // this calls SetDefaults

                // this excludes non-torch items like the NightGlow
                if (item.flame && IsTorch(item))
                    flameItems.Add(type);
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            // whenever holding an item that initially had a flame,
            // disable it if a subworld, otherwise set it back 
            if (HasFlame(item) && IsTorch(item))
                item.flame = !SubworldSystem.AnyActive<Macrocosm>();
        }
    }
}