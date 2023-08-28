using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Placeable.Paintings;

namespace Macrocosm.Content.NPCs.Global
{
	internal class VanillaNPCShop : GlobalNPC
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
