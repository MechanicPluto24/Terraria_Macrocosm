using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class ProtolithWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false; // Unsafe wall
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.ProtolithWall>();
			AddMapEntry(new Color(25, 25, 25));
		}
	}
}