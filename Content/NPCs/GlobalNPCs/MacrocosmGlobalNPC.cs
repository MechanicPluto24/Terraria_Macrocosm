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

namespace Macrocosm.NPCs.GlobalNPCs {
    public class MoonEnemyGlobalNPC : GlobalNPC {

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => EnemyCategorization.MoonEnemies.Contains(entity.type) ||
                                                                                    entity.type == NPCID.MoonLordCore;              

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            
            if(npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ActivationCore>(), 10)); // maybe do this in a separate GlobalNPC (?) - Feldy
            }

            if (EnemyCategorization.MoonEnemies.Contains(npc.type))
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonCoin>(), 10));
            }

            // if (Main.rand.NextFloat() <= 0.10f) // NOTE: What is this Activation Core even used for?
            // {
            //     if (npc.type == NPCID.MoonLordCore) {
            //         Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<ActivationCore>());
            //     }
            // }
            // if (EnemyCategorization.MoonEnemies.Contains(npc.type)) {
            //     Item.NewItem(npc.getRect(), ModContent.ItemType<MoonCoin>());
            // }
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {   
            foreach (var id in EnemyCategorization.MoonEnemies) {
                if (!SubworldSystem.IsActive<Moon>()) {
                    return;
                }
                if (SubworldSystem.IsActive<Moon>()) {
                    pool.Clear();
                    pool.Add(id, 1f);
                }
            }
        }
    }
}