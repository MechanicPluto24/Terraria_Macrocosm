using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.GlobalNPCs
{
    /// <summary> Global NPC for general NPC modifications (loot, spawn pools) </summary>
    public class MacrocosmGlobalNPC : GlobalNPC
	{
		public override void SetStaticDefaults()
		{
			SetImmunities();
		}

		/// <summary> For generic drops </summary>
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonCoin>(), 10));
			}
		}

		/// <summary> For subworld specific spawn pools </summary>
		public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			for (int id = 0; id < NPCLoader.NPCCount; id++)
			{
				if (SubworldSystem.IsActive<Moon>() && ContentSamples.NpcsByNetId[id].ModNPC is not IMoonEnemy)
 					pool.Remove(id);
 			}
		}

		private void SetImmunities()
		{
			NPCDebuffImmunityData moonEnemyDebuffData = new()
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.OnFire,
					BuffID.OnFire3,
					BuffID.CursedInferno,
					BuffID.Confused,
					BuffID.Poisoned,
					BuffID.Venom
				}
			};

			for (int id = 0; id < NPCLoader.NPCCount; id++)
			{
				if (ContentSamples.NpcsByNetId[id].ModNPC is IMoonEnemy)
					NPCID.Sets.DebuffImmunitySets.Add(id, moonEnemyDebuffData);
			}
		}
	}
}