using Macrocosm.Common.Utility;
using Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.BossSummons{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class CraterDemonSummon : ModItem{
		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Lunar Skull");
			Tooltip.SetDefault("Summons the Crater Demon\nMust be used on the Moon");
			ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
		}

		public override void SetDefaults(){
			Item.width = 20;
			Item.height = 18;
			Item.scale = 1f;
			Item.maxStack = 20;
			Item.rare = ItemRarityID.Red;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
		}

		public override bool CanUseItem(Player player)
			=> player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && NPC.downedMoonlord && !NPC.AnyNPCs(ModContent.NPCType<CraterDemon>());

		public override bool? UseItem(Player player){
			if(NPCUtils.SummonBossDirectlyWithMessage(player.Center - new Vector2(0f, 240f), ModContent.NPCType<CraterDemon>()))
				SoundEngine.PlaySound(SoundID.ForceRoar, player.position);

			return true;
		}
	}
}
