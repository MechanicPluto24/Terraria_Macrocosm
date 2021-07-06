using Macrocosm.Content.NPCs.Friendly.TownNPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Macrocosm.Content.NPCs.Friendly.TownNPCs;

namespace Macrocosm.Content.Items.Currency
{
    public class UnuCredit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Coin");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 8));
        }
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = 750;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int champ = NPC.FindFirstNPC(NPCType<Astronaut>());
            for (int iterate = 0; iterate < Main.maxNPCs; iterate++)
            {
                NPC npc = Main.npc[iterate];
                if (npc.type == NPCType<Astronaut>() && !npc.active)
                {
                    tooltips.Add(new TooltipLine(mod, "Name", "For the champ")
                    {
                        overrideColor = Color.DarkGray,
                        text = $"These are a currency for the Moon Champion, but unfortunately he is not with you"
                    });
                    return;
                }
                else if (npc.type == NPCType<Astronaut>() && npc.active)
                {
                    tooltips.Add(new TooltipLine(mod, "Name", "For the champ")
                    {
                        overrideColor = Color.DarkGray,
                        text = $"These are a currency for {Main.npc[champ].GivenName}, the Moon Champion"
                    });
                }
            }
        }
    }
}