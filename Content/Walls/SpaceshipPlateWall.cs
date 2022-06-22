using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls {
	public class SpaceshipPlateWall : ModWall {
		public override void SetStaticDefaults() {
			Main.wallHouse[Type] = false;
			ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.SpaceshipPlateWall>();
			AddMapEntry(new Color(127, 127, 127));
		}
	}
}