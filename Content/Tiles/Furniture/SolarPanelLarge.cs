﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
	public class SolarPanelLarge : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);

			HitSound = SoundID.Dig;
			DustType = -1;

			AddMapEntry(new Color(0, 52, 154), CreateMapEntryName());

            RegisterItemDrop(ModContent.ItemType<Items.Placeable.Furniture.SolarPanelLarge>(), 0, 1);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
		}

	}

}
