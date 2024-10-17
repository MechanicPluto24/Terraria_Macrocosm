using Macrocosm.Common.Sets;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class TorchGlobalItem : GlobalItem
    {
        public override void HoldItem(Item item, Player player)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                // Whenever holding an item that initially had a flame,
                // disable it if a subworld, otherwise set it back 
                if (item.flame && ItemID.Sets.Torches[item.type] && !ItemSets.AllowedTorches[item.type])
                    item.flame = false;
            }
        }
    }
}