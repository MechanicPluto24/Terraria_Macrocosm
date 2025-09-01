using Macrocosm.Content.Items.Paintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    public class VanillaNPCShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Painter && shop.Name == "Decor")
            {
                shop.Add<Freedomfaller>(Condition.MoonPhaseWaxingGibbous, Condition.DownedGolem);
            }
        }
    }
}
