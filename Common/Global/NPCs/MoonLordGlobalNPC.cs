using Macrocosm.Content.Items.Drops;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Common.Global.NPCs
{
    public class MoonLordGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.MoonLordCore;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CortexFragment>(), 10));
        }
    }
}





