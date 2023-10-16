using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	// SPRITING: Uses ExampleMod sprite as placeholder
	public class MoonBaseChestKey : ModItem
	{
		public override void SetStaticDefaults() 
		{
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GoldenKey);
		}
	}
}
