using Macrocosm.Content.NPCs.Friendly.TownNPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Currency
{
	public class MoonCoin : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
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
			int champ = NPC.FindFirstNPC(NPCType<MoonChampion>());
			for (int iterate = 0; iterate < Main.maxNPCs; iterate++)
			{
				NPC npc = Main.npc[iterate];
				if (npc.type == NPCType<MoonChampion>() && !npc.active)
				{
					tooltips.Add(new TooltipLine(Mod, "Name", "For the champ")
					{
						OverrideColor = Color.DarkGray,
						Text = $"These are a currency for the Moon Champion, but unfortunately he is not with you"
					});
					return;
				}
				else if (npc.type == NPCType<MoonChampion>() && npc.active)
				{
					tooltips.Add(new TooltipLine(Mod, "Name", "For the champ")
					{
						OverrideColor = Color.DarkGray,
						Text = $"These are a currency for {Main.npc[champ].GivenName}, the Moon Champion"
					});
				}
			}
		}
	}
}