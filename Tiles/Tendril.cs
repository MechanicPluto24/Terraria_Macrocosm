using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Tiles
{
	public class Tendril : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 300;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.Tendril>();
			AddMapEntry(new Color(188, 0, 0));
		}
	}
}