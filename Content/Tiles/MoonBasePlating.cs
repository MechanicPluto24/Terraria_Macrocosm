using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class MoonBasePlating : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = false;
			MinPick = 225;
			MineResist = 4f;
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.MoonBasePlating>();
			AddMapEntry(new Color(180, 180, 180));
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{

		}
	}
}