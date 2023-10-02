using Humanizer;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
	public class MoonBasePlating : ModTile, IHasConditionalSlopeFrames
    {
		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBrick[Type] = true;

			DustType = ModContent.DustType<MoonBasePlatingDust>();

            MinPick = 225;
            MineResist = 4f;

            AddMapEntry(new Color(180, 180, 180));
        }

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			return TileFraming.PlatingStyle(i, j);
		}

		public void ApplySlopeFrames(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			var frame = (tile.TileFrameX, tile.TileFrameY);

			if (frame is (0, 54) or (36, 54) or (72, 54))
			{
				Main.tile[i, j].TileFrameX = 324;
				Main.tile[i, j].TileFrameY = 0;
			}

			if (frame is (0, 72) or (36, 72) or (72, 72))
			{
				Main.tile[i, j].TileFrameX = 324;
				Main.tile[i, j].TileFrameY = 18;
			}

			if (frame is (18, 54) or (54, 54) or (90, 54))
			{
				Main.tile[i, j].TileFrameX = 342;
				Main.tile[i, j].TileFrameY = 0;
			}

			if (frame is (18, 72) or (54, 72) or (90, 72))
			{
				Main.tile[i, j].TileFrameX = 342;
				Main.tile[i, j].TileFrameY = 18;
			}
		}

		public bool ShouldDrawAsSloped(TileDrawInfo drawInfo)
		{
			var frame = (drawInfo.tileFrameX, drawInfo.tileFrameY);
			return frame is (324, 0) or (324, 18) or (342, 0) or (342, 18);
		}
	}
}