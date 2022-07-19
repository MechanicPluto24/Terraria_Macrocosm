using Terraria;
using SubworldLibrary;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace Macrocosm.Content.Items.GlobalItems
{
    public class TorchGlobalItem : GlobalItem
    {
        private static readonly List<int> flameItems = new();

        public static bool IsTorch(Item item) => ItemID.Sets.Torches[item.type];
        public static bool HasFlame(Item item) => flameItems.Contains(item.type);

        public override void Load()
        {
            for(int type = 0; type < ItemID.Count; type++)
            {
                Item item = new();
                item.CloneDefaults(type);
            
                if (item.flame)
                {
                    flameItems.Add(type);
                }
            }
        }

        public override void HoldItem(Item item, Player player)
        {
            if(HasFlame(item) && IsTorch(item)) // excluding Nightglow and Brand of the Inferno
            {
                item.flame = !SubworldSystem.AnyActive<Macrocosm>();
            }
        }

    }
}