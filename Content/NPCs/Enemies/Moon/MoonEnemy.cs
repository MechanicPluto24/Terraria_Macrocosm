using Macrocosm.Content.Biomes;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	/// <summary>
	/// Abstract class for every moon enemy 
	/// </summary>
	public abstract class MoonEnemy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCDebuffImmunityData debuffData = new()
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

			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
		}

		public override void SetDefaults()
		{

			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary

		}

		// done in MacrocosmGlobalNPC
		/*
        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<MoonCoin>(), 10));
        }
        */

	}
}
