using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Tiles
{
	public class SpaceshipFlooring : ModTile
	{
		public override void SetDefaults() 
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			minPick = 10000;
			mineResist = 3f;
			drop = ItemType<Items.Placeables.BlocksAndWalls.SpaceshipFlooring>();
			AddMapEntry(new Color(0, 95, 255));
		}
	}
}