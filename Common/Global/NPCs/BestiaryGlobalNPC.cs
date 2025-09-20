using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using System.Linq;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs;

public class BestiaryGlobalNPC : GlobalNPC
{
    public override void SetDefaults(NPC npc)
    {
        if (npc.ModNPC is ModNPC modNPC)
        {
            if (NPCSets.MoonNPC[npc.type])
                modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<MoonBiome>().Type).ToArray();
            //else if (NPCSets.MarsEnemies[npc.type])
            //    modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<MarsBiome>().Type).ToArray();
            else
                modNPC.SpawnModBiomes = npc.ModNPC.SpawnModBiomes.Prepend(ModContent.GetInstance<EarthBiome>().Type).ToArray();
        }
    }

    public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        if (npc.ModNPC is null)
        {
            EarthBiome earthBiome = ModContent.GetInstance<EarthBiome>();
            bestiaryEntry.Info.Add(new ModBiomeBestiaryInfoElement(Mod, earthBiome.DisplayName.Key, earthBiome.BestiaryIcon, null, null));
        }
        else if (npc.ModNPC.Mod.Name == nameof(Macrocosm)) // TML: this would make a great addition to tML 
        {
            if (!NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(npc.type, out var value) || !value.Hide)
            {
                // BestiaryFlavorText entry in the NPC localization isadded automatically for all not hidden entries.
                LocalizedText flavorText = Language.GetOrRegister("Mods.Macrocosm.NPCs." + npc.ModNPC.Name + ".BestiaryFlavorText");
                if (flavorText != LocalizedText.Empty)
                    bestiaryEntry.Info.AddRange([new FlavorTextBestiaryInfoElement(flavorText.Key)]);
            }
        }
    }
}
