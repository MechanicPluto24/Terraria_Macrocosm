using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class IrradiatedBrickWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = false;
			drop = ItemType<Content.Items.Placeables.BlocksAndWalls.IrradiatedBrickWall>();
			AddMapEntry(new Color(65, 65, 65));
		}
	}
}