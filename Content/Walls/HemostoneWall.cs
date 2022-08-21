using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class HemostoneWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.HemostoneWall>();
			AddMapEntry(new Color(65, 65, 65));
		}
	}
}