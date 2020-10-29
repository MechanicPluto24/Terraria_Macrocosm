using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Items.Materials.Chunks
{
	public class SidusChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sidus Chunk");
			Tooltip.SetDefault("'The fire burns like a star'");
			ItemID.Sets.ItemNoGravity[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 4)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.rare = ItemRarityID.Red;
			item.maxStack = 999;
		}
	}
}