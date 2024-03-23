using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture.MoonBase
{
	public class MoonBaseConsole : ModTile
	{
        private Asset<Texture2D> glowmask;
        public override void Load()
        {
            glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");
        }

        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table, 2, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);

			HitSound = SoundID.Dig;

			DustType = ModContent.DustType<MoonBasePlatingDust>();

			AddMapEntry(new Color(180, 180, 180), CreateMapEntryName());

            RegisterItemDrop(ModContent.ItemType<Items.Placeable.Furniture.MoonBase.MoonBaseConsole>(), 0, 1);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            int width = TileObjectData.GetTileData(tile).CoordinateWidth;
            int height = TileObjectData.GetTileData(tile).CoordinateHeights[tile.TileFrameY / 18 % 2];

			spriteBatch.Draw(
				glowmask.Value,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.TileFrameX, tile.TileFrameY, width, height),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f
			);
		}

	}

}
