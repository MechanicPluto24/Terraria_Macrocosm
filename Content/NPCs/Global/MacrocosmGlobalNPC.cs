using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
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
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                npc.GravityMultiplier *= MacrocosmSubworld.Current.GravityMultiplier;
                npc.GravityIgnoresSpace = true;
            }
        }

        // TML: this would make a great addition to tML 
        // BestiaryFlavorText could be added automatically for all not hidden entries. 
        // The text box will only be displayed if the text is not empty.
        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            if (npc.ModNPC is null || npc.ModNPC.Mod.Name != Macrocosm.Instance.Name)
                return;

            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(npc.type, out var value) && value.Hide)
                return;

            LocalizedText flavorText = Language.GetOrRegister("Mods.Macrocosm.NPCs." + npc.ModNPC.Name + ".BestiaryFlavorText");
            if (flavorText != LocalizedText.Empty)
            {
                bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                    new FlavorTextBestiaryInfoElement(flavorText.Key)
                });
            }
        }

        private static void SetImmunities()
        {
            for (int type = 0; type < NPCLoader.NPCCount; type++)
            {
                ModNPC npc = NPCLoader.GetNPC(type);

                if (npc is IMoonEnemy)
                {
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.OnFire] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.CursedInferno] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Frostburn] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Confused] = true;
                    NPCID.Sets.SpecificDebuffImmunity[type][BuffID.Poisoned] = true;
                }
            }
        }
    }
}