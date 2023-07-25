using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
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

		/// <summary> For common drops </summary>
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (npc.ModNPC is IMoonEnemy)
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 10));
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
			{
				npc.GravityMultiplier *= MacrocosmSubworld.Current.GravityMultiplier;
				npc.GravityIgnoresSpace = true;
			}
		}

		public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			if (npc.ModNPC is null || npc.ModNPC.Mod != Macrocosm.Instance)
				return;

			LocalizedText flavorText = Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.NPCs." + npc.ModNPC.Name + ".BestiaryFlavorText");
			if (flavorText != LocalizedText.Empty)
			{
				bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
					new FlavorTextBestiaryInfoElement(flavorText.Key)
				});
			} 
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
				ModNPC npc = NPCLoader.GetNPC(id);

				if (npc is IMoonEnemy)
					NPCID.Sets.DebuffImmunitySets[id] = moonEnemyDebuffData;
			}
		}
	}
}