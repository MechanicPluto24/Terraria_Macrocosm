using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.NPCs.Global
{
	public class MoonLordGlobalNPC : GlobalNPC
	{
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.MoonLordCore;

		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CortexFragment>(), 10));
		}

		public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.RemoveAll(e => e is FlavorTextBestiaryInfoElement); // removes the Vanilla lore text

			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement(
					"The Vortex Titan, and ruler of the Moon. His strength and abilities are matched only by his obsession with conquering Earth.")
			});

		}
	}
}





