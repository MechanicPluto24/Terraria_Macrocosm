using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Content.Items.Materials.Chunks
{
	public class NubisChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nubis Chunk");
			Tooltip.SetDefault("'It spins akin to a galaxy'");
			ItemID.Sets.ItemNoGravity[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 7)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.rare = ItemRarityID.Pink;
			item.maxStack = 999;
		}
	}
}