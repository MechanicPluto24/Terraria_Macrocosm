using Humanizer;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
	public class MoonBasePlating : ModTile, IHasCustomDraw, IHasConditionalSlopeFrames
    {
		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBrick[Type] = true;
			//TileID.Sets.GemsparkFramingTypes[Type] = Type;

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
			// Using a custom code instead of gemspark for tile framing
			return TileFraming.PlatingStyle(i, j);
		}

		public bool CustomDraw(Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, TileDrawInfo drawData, Rectangle normalTileRect, Vector2 normalTilePosition)
		{
			Tile tile = Main.tile[tileX, tileY];

			if (tile.IsSloped() && !ShouldBypassSlopeDrawing(drawData))
			{
				var slope = drawData.tileCache.Slope;
				int a = 2;
				for (int i = 0; i < 8; i++)
				{
					int b = i * -2;
					int c = 16 - i * 2;
					int d = 16 - c;
					int e;
					switch (slope)
					{
						case SlopeType.SlopeDownLeft:
							b = 0;
							e = i * 2;
							c = 14 - i * 2;
							d = 0;

							if (i is 6)
								d = 2;

							break;
						case SlopeType.SlopeDownRight:
							b = 0;
							e = 16 - i * 2 - 2;
							c = 14 - i * 2;
							d = 0;

							if (i is 6)
							{
								d += 2;
							}
		
							break;

						case SlopeType.SlopeUpLeft:
							e = i * 2;

							if (i is 6)
								d = 10;

							break;
						default:
							e = 16 - i * 2 - 2;

							if (i is 6)
								d = 10;
							break;
					}

					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(e, i * a + b), new Rectangle(drawData.tileFrameX + drawData.addFrX + e, drawData.tileFrameY + drawData.addFrY + d, a, c), drawData.finalColor, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
				}

				int f = (((int)slope <= 2) ? 14 : 0);
				Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(0f, f), new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + f, 16, 2), drawData.finalColor, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);

				return false;
			}

			return true;
		}

		public void ApplySlopeFrames(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			var slope = tile.Slope;
			var frame = (tile.TileFrameX, tile.TileFrameY);

			if (frame is (0, 54) or (36, 54) or (72, 54) && slope is SlopeType.SlopeDownRight)
			{
				Main.tile[i, j].TileFrameX = 324;
				Main.tile[i, j].TileFrameY = 0;
			}

			if (frame is (0, 72) or (36, 72) or (72, 72) && slope is SlopeType.SlopeUpRight)
			{
				Main.tile[i, j].TileFrameX = 324;
				Main.tile[i, j].TileFrameY = 18;
			}

			if (frame is (18, 54) or (54, 54) or (90, 54) && slope is SlopeType.SlopeDownLeft)
			{
				Main.tile[i, j].TileFrameX = 342;
				Main.tile[i, j].TileFrameY = 0;
			}

			if (frame is (18, 72) or (54, 72) or (90, 72) && slope is SlopeType.SlopeUpLeft)
			{
				Main.tile[i, j].TileFrameX = 342;
				Main.tile[i, j].TileFrameY = 18;
			}
		}

		public bool ShouldBypassSlopeDrawing(TileDrawInfo drawInfo)
		{
			var frame = (drawInfo.tileFrameX, drawInfo.tileFrameY);
			return frame is (324, 0) or (324, 18) or (342, 0) or (342, 18);
		}
	}
}