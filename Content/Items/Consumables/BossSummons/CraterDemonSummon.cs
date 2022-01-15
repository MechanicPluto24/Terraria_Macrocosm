using Macrocosm.Common.Utility;
using Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.BossSummons{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class CraterDemonSummon : ModItem{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Lunar Skull");
			Tooltip.SetDefault("Summons the Crater Demon\nMust be used on the Moon");
			ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
		}

		public override void SetDefaults(){
			item.width = 20;
			item.height = 18;
			item.scale = 1f;
			item.maxStack = 20;
			item.rare = ItemRarityID.Red;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.consumable = true;
		}

		public override bool CanUseItem(Player player)
			=> player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && NPC.downedMoonlord && !NPC.AnyNPCs(ModContent.NPCType<CraterDemon>());

		public override bool UseItem(Player player){
			if(NPCUtils.SummonBossDirectlyWithMessage(player.Center - new Vector2(0f, 240f), ModContent.NPCType<CraterDemon>()))
				Main.PlaySound(SoundID.ForceRoar, player.position, 0);

			return true;
		}
	}
}
