using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class SpaceshipPlate : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 10000;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.SpaceshipPlate>();
			AddMapEntry(new Color(255, 255, 255));
		}
	}
}