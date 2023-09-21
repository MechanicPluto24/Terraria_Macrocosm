using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
	public class LaunchPadMarker : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Rockets.LaunchPads.LaunchPadMarker>());
			Item.placeStyle = 3;

			Item.width = 16;
			Item.height = 16;
			Item.value = 500;
		}
	}
}
