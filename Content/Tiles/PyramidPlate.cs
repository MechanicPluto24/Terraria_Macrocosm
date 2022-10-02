using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class PyramidPlate : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			MinPick = 300;
			MineResist = 3f;
			ItemDrop = ItemType<Content.Items.Placeable.BlocksAndWalls.PyramidPlate>();
			AddMapEntry(new Color(22, 22, 22));
		}
	}
}