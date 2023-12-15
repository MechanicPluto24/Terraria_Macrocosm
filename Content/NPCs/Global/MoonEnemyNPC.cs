using Macrocosm.Common.DropRules;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Global
{
	public class MoonEnemyNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public override void SetDefaults(NPC entity)
		{
			if(entity.ModNPC is IMoonEnemy)
                entity.ModNPC.SpawnModBiomes = entity.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<MoonBiome>().Type).ToArray();
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.ModNPC is IMoonEnemy moonEnemy && !moonEnemy.DropMoonstone)
                return;

            npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Moonstone>(), 10, 1, 5, new SubworldDropCondition<Moon>(canShowInBestiary: npc.ModNPC is IMoonEnemy)));
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

    }
}