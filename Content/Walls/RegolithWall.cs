using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class RegolithWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false; // Unsafe wall
			ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.RegolithWall>();
			AddMapEntry(new Color(30, 30, 30));
		}
	}
}