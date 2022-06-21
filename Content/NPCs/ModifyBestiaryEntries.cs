using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;

namespace Macrocosm.Content.NPCs
{

    // public class ChangeBestiaryEntries : ModSystem
    // {
    //     // TODO: iterate a dictionary of (int ID, string descriptions) when this will affect more NPCs - Feldy 
    //     public override void OnWorldLoad()
    //     {
    //         BestiaryEntry bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(NPCID.MoonLordCore);
    // 
    //         foreach (IBestiaryInfoElement element in bestiaryEntry.Info)
    //         {
    //             bestiaryEntry.Info.RemoveAll(e => (e.GetType() == typeof(FlavorTextBestiaryInfoElement))); // removes the Vanilla lore text
    //         }
    // 
    //         bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
    //         {
    //             new FlavorTextBestiaryInfoElement(
    //                 "The Vortex Titan, and ruler of the Moon. His strength and abilities are matched only by his obsession with conquering Earth.")
    //         });
    //     }
    // }
}
