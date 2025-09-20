using Macrocosm.Content.Items.Connectors;
using Macrocosm.Content.Items.Paintings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs;

/// <summary>
/// NPC vendors' default <see cref="AbstractNPCShop.Name"/> is <c>"Shop"</c>
/// <br/> Painter has an additional <c>"Decor"</c> shop
/// </summary>
public class VanillaNPCShop : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        switch (shop.NpcType)
        {
            case NPCID.Painter when shop.Name == "Decor":
                shop.Add<Freedomfaller>(Condition.MoonPhaseWaxingGibbous, Condition.DownedGolem);
                break;

            case NPCID.Steampunker:
                shop.Add<Conveyor>();
                shop.Add<ConveyorInlet>();
                shop.Add<ConveyorOutlet>();
                break;
        }
    }
}
