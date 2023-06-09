using Macrocosm.Common.Subworlds;
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

namespace Macrocosm.Content.NPCs.Global
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

		public override void AI(NPC npc)
		{
			if (MacrocosmSubworld.AnyActive)
				npc.GravityMultiplier *= MacrocosmSubworld.Current.GravityMultiplier;
		}

		private static void SetImmunities()
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
				if (NPCLoader.GetNPC(id) is IMoonEnemy)
					NPCID.Sets.DebuffImmunitySets.Add(id, moonEnemyDebuffData);
			}
		}
	}
}