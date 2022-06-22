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
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 999;
		}
	}
}