using System;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
	internal class MoonBaseSofa : ModTile
	{
		// TODO: sitting for sofas
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(253, 221, 3), Language.GetText("MapObject.Bench"));

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
			AdjTiles = new int[] { TileID.Benches };
			DustType = ModContent.DustType<MoonBasePlatingDust>();
		}

		// Draw extra set of pixels if there's no neighboring sofa to the right
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Tile tileRight = Main.tile[i+1, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			if (tile.TileType == Type)
			{
				// If on last frame and there's no neighbor sofa...
				if(tile.TileFrameX / 18 == 2 && tileRight.TileType != Type)
				{
					var data = TileObjectData.GetTileData(Type, 0);
					if(tile.TileFrameY / 18 == 0)
						spriteBatch.Draw(
							ModContent.Request<Texture2D>("Macrocosm/Content/Tiles/Furniture/MoonBase/MoonBaseSofa_Extra").Value,
							new Vector2((i + 1) * 16f, j * 16f) - Main.screenPosition + zero,
							new Rectangle(0, 0, 2, data.CoordinateHeights[0]),
							Lighting.GetColor(i + 1, j)
							);
					else if (tile.TileFrameY / 18 == 1)
						spriteBatch.Draw(
							ModContent.Request<Texture2D>("Macrocosm/Content/Tiles/Furniture/MoonBase/MoonBaseSofa_Extra").Value,
							new Vector2((i + 1) * 16f, j * 16f) - Main.screenPosition + zero,
							new Rectangle(0, data.CoordinateHeights[0] + data.CoordinatePadding, 2, data.CoordinateHeights[1]),
							Lighting.GetColor(i + 1, j)
							);
				}
			}

			return true;
		}
	}
}
