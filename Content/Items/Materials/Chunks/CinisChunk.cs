using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace Macrocosm.Content.Items.Materials.Chunks
{
	public class CinisChunk : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Cinis Chunk");
            Tooltip.SetDefault("'The core in the middle holds many universes inside'");
            ItemID.Sets.ItemNoGravity[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 6)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults() 
		{
			item.width = 30;
			item.height = 30;
			item.rare = ItemRarityID.Cyan;
			item.maxStack = 999;
		}
	}
}