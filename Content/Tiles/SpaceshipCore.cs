using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class SpaceshipCore : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			MinPick = 10000;
			MineResist = 3f;
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.SpaceshipCore>();
			AddMapEntry(new Color(176, 255, 133));
		}
	}
}