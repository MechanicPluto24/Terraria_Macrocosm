using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    /// <summary> Global NPC for general NPC modifications (loot, spawn pools) </summary>
    public class MacrocosmGlobalNPC : GlobalNPC
    {
        /// <summary> For subworld specific spawn pools </summary>
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            for (int type = 0; type < NPCLoader.NPCCount; type++)
            {
                if (SubworldSystem.IsActive<Moon>() && !NPCSets.MoonNPC[type]) { pool.Remove(type); }
                //if (SubworldSystem.IsActive<Mars>() && !NPCSets.MarsNPCs[type]) { pool.Remove(type); }
                //...
            }
            for (int type = 0; type < NPCLoader.NPCCount; type++)
            {
                if (SubworldSystem.IsActive<EarthOrbitSubworld>()) { pool.Remove(type); }
                
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!SubworldSystem.AnyActive<Macrocosm>())
                return;

            if (player.InModBiome<MoonUndergroundBiome>())
            {
                spawnRate = (int)(spawnRate * 0.75f);
                maxSpawns = (int)(maxSpawns * 1.5f);
            }
            /*
            else if (player.InModBiome<IrradiationBiome>())
            {
                //...
            }
            */
            else if (player.InModBiome<MoonNightBiome>())
            {
                spawnRate = (int)(spawnRate * 0.6f);
                maxSpawns = (int)(maxSpawns * 2f);
            }
            else if (player.InModBiome<MoonBiome>())
            {
                spawnRate = (int)(spawnRate * 1f);
                maxSpawns = (int)(maxSpawns * 1f);
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
                bestiaryEntry.Info.AddRange([new FlavorTextBestiaryInfoElement(flavorText.Key)]);
            }
        }
    }
}