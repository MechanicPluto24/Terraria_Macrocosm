using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Dusts;

namespace Macrocosm.Tiles
{
	public class Protolith : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileMerge[Type][ModContent.TileType<Regolith>()] = true;
			minPick = 225;
			mineResist = 3f;
			drop = ModContent.ItemType<Items.Placeables.BlocksAndWalls.Protolith>();
			AddMapEntry(new Color(65, 65, 65));
			soundType = SoundID.Tink;
		}
		public override bool CreateDust(int i, int j, ref int type)
		{
			type = Dust.NewDust(new Vector2(i, j).ToWorldCoordinates(), 16, 16, ModContent.DustType<ProtolithDust>());
			return false;
		}
	}
}