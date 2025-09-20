using Macrocosm.Common.Loot.DropConditions;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs;

public class LootGlobalNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        // Moonstone drops
        npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Moonstone>(), 10, 1, 5, new ConditionsChain.All(
            new SubworldDropCondition<Moon>(),
            new GlobalItemDropCondition(),
            new Condition(LocalizedText.Empty, () => !NPCSets.NoMoonstoneDrop[npc.type]).ToDropCondition(default)
        )));

        //if(npc.type == NPCID.MoonLordCore)
        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CortexFragment>(), 10));
    }
}
