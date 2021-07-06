using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class PyramidPlate : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 300;
			mineResist = 3f;
			drop = ItemType<Content.Items.Placeables.BlocksAndWalls.PyramidPlate>();
			AddMapEntry(new Color(22, 22, 22));
		}
	}
}