using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class RegolithBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			MinPick = 225;
			MineResist = 3f;
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.RegolithBrick>();
			AddMapEntry(new Color(65, 65, 65));
		}
	}
}