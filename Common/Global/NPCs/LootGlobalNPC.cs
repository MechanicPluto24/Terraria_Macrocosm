using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    public class LootGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Make all DropsMoonstone NPCs drop Moonstone while on the Moon
            if (NPCSets.DropsMoonstone[npc.type])
                npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Moonstone>(), 10, 1, 5, new SubworldDropCondition<Moon>(canShowInBestiary: true)));

            //if(npc.type == NPCID.MoonLordCore)
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CortexFragment>(), 10));
        }
    }
}
