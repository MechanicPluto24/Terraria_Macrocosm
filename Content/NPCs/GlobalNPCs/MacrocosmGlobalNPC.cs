using Macrocosm.Content.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Systems;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Macrocosm.Content.NPCs.Unfriendly.Enemies.Moon;

namespace Macrocosm.NPCs.GlobalNPCs
{
    public class MacrocosmGlobalNPC : GlobalNPC { 

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