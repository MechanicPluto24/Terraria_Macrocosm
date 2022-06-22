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

namespace Macrocosm.NPCs.GlobalNPCs {
    public class MacrocosmGlobalNPC : GlobalNPC { 

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (EnemyCategorization.MoonEnemies.Contains(npc.type))
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
                    if (!EnemyCategorization.MoonEnemies.Contains(id))
                    {
                        pool.Remove(id);
                    }
                }
            }
        }
    }
}