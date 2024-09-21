using Macrocosm.Common.Sets;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class SeedGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (SubworldSystem.AnyActive<Macrocosm>()) // TODO: Add check, if airless environment
            {
                // Grass seeds - typically placed on dirt or mud
                // Modded seeds might not add their grass seeds to this set!
                if (ItemID.Sets.GrassSeeds[item.type]) 
                    return false;

                // Flower packets
                if (ItemID.Sets.flowerPacketInfo[item.type] is not null)
                    return false;

                // Alchemy and other plants
                if (ItemSets.PlantSeed[item.type])
                    return false;

                // Tree seeds (Acorn, Sakura, )
                if (ItemSets.TreeSeed[item.type])
                    return false;
            }

            return true;
        }
    }
}