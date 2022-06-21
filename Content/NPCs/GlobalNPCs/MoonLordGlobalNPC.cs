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
    public class MoonLordGlobalNPC : GlobalNPC {

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.MoonLordCore;              

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ActivationCore>(), 10)); 
        }


        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.RemoveAll(e => (e.GetType() == typeof(FlavorTextBestiaryInfoElement))); // removes the Vanilla lore text

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "The Vortex Titan, and ruler of the Moon. His strength and abilities are matched only by his obsession with conquering Earth.")
            });

        }
    }
}





