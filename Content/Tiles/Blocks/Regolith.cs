using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
	public class Regolith : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
			Main.tileMerge[Type][ModContent.TileType<IrradiatedRock>()] = true;
			MinPick = 225;
			MineResist = 3f;
			AddMapEntry(new Color(220, 220, 220));
			HitSound = SoundID.Dig;
		}

		public override bool HasWalkDust()
		{
			return true;
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			type = Dust.NewDust(new Vector2(i, j).ToWorldCoordinates(), 16, 16, ModContent.DustType<RegolithDust>());
			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			return true;
		}
	}
}