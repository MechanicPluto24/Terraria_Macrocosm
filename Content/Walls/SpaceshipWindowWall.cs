using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Walls
{
	public class SpaceshipWindowWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = false;
			drop = ItemType<Content.Items.Placeables.BlocksAndWalls.SpaceshipWindowWall>();
			AddMapEntry(new Color(0, 178, 209));
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.5f;
			g = 0.75f;
			b = 1f;
		}
	}
}