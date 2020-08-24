using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Walls
{
	public class RegolithWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = false;
			drop = ItemType<Items.Placeables.BlocksAndWalls.RegolithWall>();
			AddMapEntry(new Color(65, 65, 65));
		}
	}
}