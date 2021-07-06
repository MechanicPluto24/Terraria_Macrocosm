using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class RegolithWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = false; // Unsafe wall
			drop = ItemType<Content.Items.Placeables.BlocksAndWalls.RegolithWall>();
			AddMapEntry(new Color(30, 30, 30));
		}
	}
}