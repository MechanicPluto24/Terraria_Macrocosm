using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Macrocosm.Content.Subworlds.Moon;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.NPCs.GlobalNPCs
{
	/// <summary>
	/// Global NPC for general NPC modifications (loot, spawn pools)
	/// </summary>
	public class MacrocosmGlobalNPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (npc.ModNPC is MoonEnemy)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonCoin>(), 10));
			}
		}

		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				for (int id = 0; id < NPCLoader.NPCCount; id++)
				{
					if (ContentSamples.NpcsByNetId[id].ModNPC is not MoonEnemy)
					{
						pool.Remove(id);
					}
				}

			}
		}
	}
}