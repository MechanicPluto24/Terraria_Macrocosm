using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Tiles
{
	public class RegolithBrick : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 225;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.RegolithBrick>();
			AddMapEntry(new Color(65, 65, 65));
		}
	}
}