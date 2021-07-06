using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Content.Items.Materials.Chunks
{
	public class TurbenChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Turben Chunk");
			Tooltip.SetDefault("'Emitting enough light to blind'");
			ItemID.Sets.ItemNoGravity[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 11)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.rare = ItemRarityID.Blue;
			item.maxStack = 999;
		}
	}
}