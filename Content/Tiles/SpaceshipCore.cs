using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class SpaceshipCore : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 10000;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.SpaceshipCore>();
			AddMapEntry(new Color(176, 255, 133));
		}
	}
}