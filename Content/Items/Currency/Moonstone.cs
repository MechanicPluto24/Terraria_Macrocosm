using Macrocosm.Content.NPCs.Friendly.TownNPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Currency
{
    public class Moonstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 750;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int champ = NPC.FindFirstNPC(ModContent.NPCType<MoonChampion>());
            for (int iterate = 0; iterate < Main.maxNPCs; iterate++)
            {
                NPC npc = Main.npc[iterate];
                if (npc.type == ModContent.NPCType<MoonChampion>() && !npc.active)
                {
                    tooltips.Add(new TooltipLine(Mod, "Name", "ForTheChamp")
                    {
                        OverrideColor = Color.DarkGray,
                        Text = Language.GetTextValue("Mods.Macrocosm.Items.Moonstone.TooltipNoChampion")
                    });
                    return;
                }
                else if (npc.type == ModContent.NPCType<MoonChampion>() && npc.active)
                {
                    tooltips.Add(new TooltipLine(Mod, "Name", "ForTheChamp")
                    {
                        OverrideColor = Color.White,
                        Text = Language.GetText("Mods.Macrocosm.Items.Moonstone.TooltipChampion").Format(Main.npc[champ].GivenName)
                    });
                }
            }
        }
    }
}