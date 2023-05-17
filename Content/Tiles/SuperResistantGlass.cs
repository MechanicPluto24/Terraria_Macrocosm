using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Tiles
{
	public class SuperResistantGlass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = false;
			MinPick = 225;
			MineResist = 4f;
			ItemDrop = ItemType<Items.Placeable.BlocksAndWalls.SuperResistantGlass>();
			AddMapEntry(new Color(50, 50, 50));
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{

		}
	}
}