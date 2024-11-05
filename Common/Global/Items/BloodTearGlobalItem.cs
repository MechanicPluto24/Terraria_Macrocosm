using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class BloodTearGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (SubworldSystem.AnyActive<Macrocosm>()) // TODO: Add check, if airless environment
            {
                if (item.type == ItemID.BloodMoonStarter)
                    return false;

            }

            return true;
        }
    }
}