using Macrocosm.Content.Items.Placeable.Paintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Global
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
