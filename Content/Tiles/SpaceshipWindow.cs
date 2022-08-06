using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class SpaceshipWindow : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = false;
			MinPick = 10000;
			MineResist = 3f;
			ItemDrop = ItemType<Items.Placeables.BlocksAndWalls.SpaceshipWindow>();
			AddMapEntry(new Color(109, 233, 255));
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.5f;
			g = 0.75f;
			b = 1f;
		}
	}
}