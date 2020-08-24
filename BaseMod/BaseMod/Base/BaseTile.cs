using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader;
using Macrocosm;

namespace Macrocosm
{
    public class BaseTile
    {
        //------------------------------------------------------//
        //-------------------BASE TILE CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods dealing with tiles, except          //
        // generation. (for that, see BaseWorldGen/BaseGoreGen) //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

		public static void AddMapEntry(ModTile tile, Color color)
		{
			tile.AddMapEntry(color);			
		}		
		
		public static void AddMapEntry(ModTile tile, Color color, string name)
		{
			ModTranslation name2 = tile.CreateMapEntryName();
			name2.SetDefault(name);
			tile.AddMapEntry(color, name2);			
		}
		
		 #region JustChestyThings
		 public static Chest GetClosestVanillaChest(Vector2 origin, float distance, int chestStyle = -1, int special = -1)
		 {
			 return GetClosestVanillaChest(origin, distance, chestStyle == -1 ? default(int[]) : new int[] { chestStyle }, special);
		 }

		 public static Chest GetClosestVanillaChest(Vector2 origin, float distance, int[] chestStyles = default(int[]), int special = -1)
		 {
			 Chest[] chests = GetVanillaChestsNear(origin, distance, chestStyles, special);
			 if (chests.Length == 0) { return null; }
			 Chest current = null;
			 foreach (Chest chest in chests)
			 {
				 float dist = Vector2.Distance(origin, new Vector2(chest.x * 16, chest.y * 16));
				 if(distance == -1 || dist < distance)
				 {
					 distance = dist; current = chest;
				 }
			 }
			 return current;
		 }


		 public static Chest[] GetVanillaChestsNear(Vector2 origin, float distance, int chestStyle = -1, int special = -1)
		 {
			 return GetVanillaChestsNear(origin, distance, chestStyle == -1 ? default(int[]) : new int[] { chestStyle }, special);
		 }

		 public static Chest[] GetVanillaChestsNear(Vector2 origin, float distance, int[] chestStyles = default(int[]), int special = -1)
		 {
			 List<Chest> chests = new List<Chest>();
			 for (int m = 0; m < Main.chest.Length; m++)
			 {
				 Chest chest = Main.chest[m];
				 if (chest == null) { continue; }
				 int x = chest.x; int y = chest.y;
				 if (distance != -1 && Vector2.Distance(origin, new Vector2((x * 16f) + 8f, (y * 16f) + 8f)) > distance){ continue; }
				 Tile tile = Main.tile[x, y];
				 if (tile == null || !tile.active() || tile.type != 21) { continue; } //if not a vanilla chest, ignore it
				 if (chestStyles == default(int[]) || IsVanillaChestOfStyle(chest, chestStyles))
				 {
					 if (special == -1) 
					 {
						 chests.Add(chest);
					 }else
					 {
						 if (tile.frameY == 0) { y += 1; }
						 if (y + 1 > Main.maxTilesY) { continue; }
						 if (special == 0 && BaseUtility.InArray(BaseConstants.TILEIDS_DUNGEONSTRICT, Main.tile[x, y + 1].type)) //dungeon
						 {
							 chests.Add(chest);
						 }
					 }
				 }
			 }
			 return chests.ToArray();
		 }

		 public static Chest[] GetVanillaChests(int minY, int maxY, int chestStyle = -1, int special = -1)
		 {
			 return GetVanillaChests(minY, maxY, chestStyle == -1 ? default(int[]) : new int[] { chestStyle }, special);
		 }

         /*
          * Returns an array of chests that are in the world given the requirements.
          * 
          * minY : the minimum tile Y to get chests.
          * maxY : the maximum tile Y to get chests.
          * chestStyles : the styles of chest to find. default(int[]) means all chest styles.
          * special : extra requirements.
          *           -1 > no special requirements.
          *            0 > needs a dungeon tile below it.
          *            1 > needs a sky island brick tile below it.
          */
        public static Chest[] GetVanillaChests(int minY, int maxY, int[] chestStyles = default(int[]), int special = -1)
        {
            System.Collections.Generic.List<Chest> chests = new System.Collections.Generic.List<Chest>();
            for(int m = 0; m < Main.chest.Length; m++)
            {
                Chest chest = Main.chest[m];
                if (chest == null){ continue; }
                int x = chest.x; int y = chest.y;
                if (y < minY || y > maxY) { continue; }
                Tile tile = Main.tile[x, y];
                if (tile == null || !tile.active() || tile.type != 21) { continue; } //if not a vanilla chest, ignore it
                if (chestStyles == default(int[]) || IsVanillaChestOfStyle(chest, chestStyles))
                {
                    if (special == -1) { chests.Add(chest); }else
                    {
                        if (tile.frameY == 0) { y += 1; }
                        if(y + 1 > Main.maxTilesY){ continue; }
                        if (special == 0 && BaseUtility.InArray(BaseConstants.TILEIDS_DUNGEONSTRICT, Main.tile[x, y + 1].type)) //dungeon
                        {
                            chests.Add(chest);
                        }
                    }
                }
            }
            return chests.ToArray();
        }

		public static bool IsVanillaChestOfStyle(Chest chest, int chestStyle) { return IsVanillaChestOfStyle(chest.x, chest.y, chestStyle); }
		public static bool IsVanillaChestOfStyle(int x, int y, int chestStyle) { return IsVanillaChestOfStyle(x, y, new int[] { chestStyle }); }

        public static bool IsVanillaChestOfStyle(Chest chest, int[] chestStyles) { return IsVanillaChestOfStyle(chest.x, chest.y, chestStyles); }

        /*
         * Returns true if the vanilla chest at the given coordinates is of the style provided.
         */
        public static bool IsVanillaChestOfStyle(int x, int y, int[] chestStyles)
        {
            x = Math.Max(0, Math.Min(Main.maxTilesX, x));
            y = Math.Max(0, Math.Min(Main.maxTilesY, y));
            Tile tile = Main.tile[x, y];
            if(tile != null && tile.active() && tile.type == 21)
            {
				foreach (int chestStyle in chestStyles)
				{
					if (tile.frameX == (short)(36 * chestStyle) || tile.frameX == (short)(36 * chestStyle) + 18)
					{
						return true;
					}
				}
            }
            return false;
        }

        public static bool IsVanillaChestLocked(Chest chest)
        {
            return IsVanillaChestLocked(chest.x, chest.y);
        }

        /*
         * Returns true if the golden or shadow chest at the given coordinates is locked.
         */
        public static bool IsVanillaChestLocked(int x, int y)
        {
            for (int x2 = x; x2 <= x + 1; x2++)
            {
                for (int y2 = y; y2 <= y + 1; y2++)
                {
                    if (Main.tile[x2, y2] == null){ Main.tile[x2, y2] = new Tile(); }
                    if ((Main.tile[x2, y2].frameX >= 72 && Main.tile[x2, y2].frameX <= 106) || (Main.tile[x2, y2].frameX >= 144 && Main.tile[x2, y2].frameX <= 178))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

#endregion


		public static void SetTileFrame(int x, int y, int tileWidth, int tileHeight, int frame, int tileFrameWidth = 16)
		{
			int type = Main.tile[x, y].type;
			int frameWidth = (tileFrameWidth + 2) * tileWidth;
			for (int x1 = 0; x1 < tileWidth; x1++)
			{
				for (int y1 = 0; y1 < tileHeight; y1++)
				{
					int x2 = x + x1; int y2 = y + y1;
					Main.tile[x2, y2].frameX = (short)((frame * frameWidth) + ((tileFrameWidth + 2) * x1));
				}
			}
		}

		/*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
		public static Vector2 GetClosestTile(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
		{
			Vector2 originalPos = new Vector2(x, y);
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			Vector2 pos = default(Vector2);
			float dist = -1;
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.active() && tile.type == type && (addTile == null || addTile(tile)) && (dist == -1 || Vector2.Distance(originalPos, new Vector2(x1, y1)) < dist))
					{
						dist = Vector2.Distance(originalPos, new Vector2(x1, y1));
                        if (type == 21 || (TileObjectData.GetTileData(tile.type, 0) != null && (TileObjectData.GetTileData(tile.type, 0).Width > 1 || TileObjectData.GetTileData(tile.type, 0).Height > 1)))
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= (tile.frameX / 18) % 2;
								y2 -= (tile.frameY / 18) % 2;
							}else
							{
								Vector2 top = FindTopLeft(x2, y2);
                                x2 = (int)top.X; y2 = (int)top.Y;
							}
							pos = new Vector2(x2, y2);
						}else
						{
							pos = new Vector2(x1, y1);
						}
					}
				}
			}
			return pos;
		}

        public static Point FindTopLeftPoint(int x, int y)
        {
            Vector2 v2 = FindTopLeft(x, y);
            return new Point((int)v2.X, (int)v2.Y);
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y]; if (tile == null) return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
            x -= (tile.frameX / 18) % data.Width;
            y -= (tile.frameY / 18) % data.Height;
            return new Vector2(x, y);
        }

		/*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
		public static Vector2[] GetTiles(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
		{
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			List<Vector2> tilePos = new List<Vector2>();
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.active() && tile.type == type && (addTile == null || addTile(tile)))
					{
						if (type == 21 || TileObjectData.GetTileData(tile).Width > 1 || TileObjectData.GetTileData(tile).Height > 1)
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= (tile.frameX / 18) % 2;
								y2 -= (tile.frameY / 18) % 2;
							}else
							{
								Point p = FindTopLeftPoint(x2, y2); x2 = p.X; y2 = p.Y;
							}
							Vector2 topLeft = new Vector2(x2, y2);
							if (tilePos.Contains(topLeft)) { continue; }
							tilePos.Add(topLeft);
						}else
						{
							tilePos.Add(new Vector2(x1, y1));
						}
					}
				}
			}
			return tilePos.ToArray();
		}

		/*
         * Returns the total count of the given liquid within the distance provided.
         */
		public static int LiquidCount(int x, int y, int distance = 25, int liquidType = 0)
		{
			int liquidAmt = 0;
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.liquid > 0 && (liquidType == 0 ? tile.water() : liquidType == 1 ? tile.lava() : tile.honey()))
					{
						liquidAmt += tile.liquid;
					}
				}
			}
			return liquidAmt;
		}

		/*
         * Returns true if the tile type acts similarly to a platform.
         */
		public static bool IsPlatform(int type)
		{
			return Main.tileSolid[type] && Main.tileSolidTop[type];
		}

        public static bool AlchemyFlower(int type) { return type == 82 || type == 83 || type == 84; }

        /*
         * Returns the dust type the tile type emits when mined. If the dust type is not found (or the tile has no dust), it returns -1.
         * If the tile at the position is null or inactive, it returns -1.
         */
        public static int GetTileDust(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return -1;
            return GetTileDust(tile.type, tile.frameX, tile.frameY);
        }

        /*
         * Returns the dust type the tile type emits when mined. If the dust type is not found (or the tile has no dust), it returns -1.
         * frameX : the x frame of the tile.
         * frameY : the y frame of the tile.
         */
        public static int GetTileDust(int type, int frameX = 0, int frameY = 0)
        {
            #region GetTileDust
			int dustType = 0;
			if (type == 216)
			{
				dustType = -1;
			}
			if (type == 335)
			{
				dustType = -1;
			}
			if (type == 338)
			{
				dustType = -1;
			}
			if (type == 0)
			{
				dustType = 0;
			}
			if (type == 192)
			{
				dustType = 3;
			}
			if (type == 208)
			{
				dustType = 126;
			}
			if (type == 16)
			{
				dustType = 1;
				if (frameX >= 36)
				{
					dustType = 82;
				}
			}
			if (type == 1 || type == 17 || type == 38 || type == 39 || type == 41 || type == 43 || type == 44 || type == 48 || Main.tileStone[(int)type] || type == 85 || type == 90 || type == 92 || type == 96 || type == 97 || type == 99 || type == 117 || type == 130 || type == 131 || type == 132 || type == 135 || type == 135 || type == 142 || type == 143 || type == 144 || type == 210 || type == 207 || type == 235 || type == 247 || type == 272 || type == 273 || type == 283)
			{
				dustType = 1;
			}
			if (type == 311)
			{
				dustType = 207;
			}
			if (type == 312)
			{
				dustType = 208;
			}
			if (type == 313)
			{
				dustType = 209;
			}
			if (type == 104)
			{
				dustType = -1;
			}
			if (type == 95 || type == 98 || type == 100 || type == 174 || type == 173)
			{
				dustType = 6;
			}
			if (type == 30 || type == 86 || type == 94 || type == 106 || type == 114 || type == 124 || type == 128 || type == 269)
			{
				dustType = 7;
			}
			if (type == 334)
			{
				dustType = 7;
			}
			if (type <= 89)
			{
				switch (type)
				{
					case 10:
					case 11:
						return -1;
					default:
						switch (type)
						{
							case 87:
							case 89:
								return -1;
						}
						break;
				}
			}
			else
			{
				if (type == 93 || type == 139)
				{
					return -1;
				}
				switch (type)
				{
					case 319:
					case 320:
						return -1;
				}
			}
			if (type == 240)
			{
				int num15 = (int)(frameX / 54);
				if (frameY >= 54)
				{
					num15 += 36;
				}
				dustType = 7;
				if (num15 == 16 || num15 == 17)
				{
					dustType = 26;
				}
				if (num15 >= 46 && num15 <= 49)
				{
					dustType = -1;
				}
			}
			if (type == 241)
			{
				dustType = 1;
			}
			if (type == 242)
			{
				dustType = -1;
			}
			if (type == 246)
			{
				dustType = -1;
			}
			if (type == 36)
			{
				dustType = -1;
			}
			if (type == 170)
			{
				dustType = 196;
			}
			if (type == 315)
			{
				dustType = 225;
			}
			if (type == 171)
			{
				if (Main.rand.Next(2) == 0)
				{
					dustType = 196;
				}
				else
				{
					dustType = -1;
				}
			}
			if (type == 326)
			{
				dustType = 13;
			}
			if (type == 327)
			{
				dustType = 13;
			}
			if (type == 336)
			{
				dustType = 6;
			}
			if (type == 328)
			{
				dustType = 13;
			}
			if (type == 329)
			{
				dustType = 13;
			}
			if (type == 330)
			{
				dustType = 9;
			}
			if (type == 331)
			{
				dustType = 11;
			}
			if (type == 332)
			{
				dustType = 19;
			}
			if (type == 333)
			{
				dustType = 11;
			}
			if (type == 101)
			{
				dustType = -1;
			}
			if (type == 19)
			{
				int num16 = (int)(frameY / 18);
				if (num16 == 0 || num16 == 9 || num16 == 10 || num16 == 11 || num16 == 12)
				{
					dustType = 7;
				}
				else if (num16 == 1)
				{
					dustType = 77;
				}
				else if (num16 == 2)
				{
					dustType = 78;
				}
				else if (num16 == 3)
				{
					dustType = 79;
				}
				else if (num16 == 4)
				{
					dustType = 26;
				}
				else if (num16 == 5)
				{
					dustType = 126;
				}
				else if (num16 == 13)
				{
					dustType = 109;
				}
				else if (num16 == 14)
				{
					dustType = 13;
				}
				else if (num16 >= 15 || num16 <= 16)
				{
					dustType = -1;
				}
				else if (num16 == 17)
				{
					dustType = 215;
				}
				else if (num16 == 18)
				{
					dustType = 214;
				}
				else
				{
					dustType = 1;
				}
			}
			if (type == 79)
			{
				int num17 = (int)(frameY / 36);
				if (num17 == 0)
				{
					dustType = 7;
				}
				else if (num17 == 1)
				{
					dustType = 77;
				}
				else if (num17 == 2)
				{
					dustType = 78;
				}
				else if (num17 == 3)
				{
					dustType = 79;
				}
				else if (num17 == 4)
				{
					dustType = 126;
				}
				else if (num17 == 8)
				{
					dustType = 109;
				}
				else if (num17 >= 9)
				{
					dustType = -1;
				}
				else
				{
					dustType = 1;
				}
			}
			if (type == 18)
			{
				int num18 = (int)(frameX / 36);
				if (num18 == 0)
				{
					dustType = 7;
				}
				else if (num18 == 1)
				{
					dustType = 77;
				}
				else if (num18 == 2)
				{
					dustType = 78;
				}
				else if (num18 == 3)
				{
					dustType = 79;
				}
				else if (num18 == 4)
				{
					dustType = 26;
				}
				else if (num18 == 5)
				{
					dustType = 40;
				}
				else if (num18 == 6)
				{
					dustType = 5;
				}
				else if (num18 == 7)
				{
					dustType = 26;
				}
				else if (num18 == 8)
				{
					dustType = 4;
				}
				else if (num18 == 9)
				{
					dustType = 126;
				}
				else if (num18 == 10)
				{
					dustType = 148;
				}
				else if (num18 == 11 || num18 == 12 || num18 == 13)
				{
					dustType = 1;
				}
				else if (num18 == 14)
				{
					dustType = 109;
				}
				else if (num18 == 15)
				{
					dustType = 126;
				}
				else
				{
					dustType = -1;
				}
			}
			if (type == 14 || type == 87 || type == 88)
			{
				dustType = -1;
			}
			if (type >= 255 && type <= 261)
			{
				int num19 = (int)(type - 255);
				dustType = 86 + num19;
				if (num19 == 6)
				{
					dustType = 138;
				}
			}
			if (type >= 262 && type <= 268)
			{
				int num20 = (int)(type - 262);
				dustType = 86 + num20;
				if (num20 == 6)
				{
					dustType = 138;
				}
			}
			if (type == 178)
			{
				int num21 = (int)(frameX / 18);
				dustType = 86 + num21;
				if (num21 == 6)
				{
					dustType = 138;
				}
			}
			if (type == 186)
			{
				if (frameX <= 360)
				{
					dustType = 26;
				}
				else if (frameX <= 846)
				{
					dustType = 1;
				}
				else if (frameX <= 954)
				{
					dustType = 9;
				}
				else if (frameX <= 1062)
				{
					dustType = 11;
				}
				else if (frameX <= 1170)
				{
					dustType = 10;
				}
				else if (frameX <= 1332)
				{
					dustType = 0;
				}
				else if (frameX <= 1386)
				{
					dustType = 10;
				}
				else
				{
					dustType = 80;
				}
			}
			if (type == 187)
			{
				if (frameX <= 144)
				{
					dustType = 1;
				}
				else if (frameX <= 306)
				{
					dustType = 38;
				}
				else if (frameX <= 468)
				{
					dustType = 36;
				}
				else if (frameX <= 738)
				{
					dustType = 30;
				}
				else if (frameX <= 970)
				{
					dustType = 1;
				}
				else if (frameX <= 1132)
				{
					dustType = 148;
				}
				else if (frameX <= 1132)
				{
					dustType = 155;
				}
				else if (frameX <= 1348)
				{
					dustType = 1;
				}
				else if (frameX <= 1564)
				{
					dustType = 0;
				}
			}
			if (type == 105)
			{
				dustType = 1;
				if (frameX >= 1548 && frameX <= 1654)
				{
					dustType = 148;
				}
			}
			if (type == 337)
			{
				dustType = 1;
			}
			if (type == 239)
			{
				int num22 = (int)(frameX / 18);
				if (num22 == 0)
				{
					dustType = 9;
				}
				if (num22 == 1)
				{
					dustType = 81;
				}
				if (num22 == 2)
				{
					dustType = 8;
				}
				if (num22 == 3)
				{
					dustType = 82;
				}
				if (num22 == 4)
				{
					dustType = 11;
				}
				if (num22 == 5)
				{
					dustType = 83;
				}
				if (num22 == 6)
				{
					dustType = 10;
				}
				if (num22 == 7)
				{
					dustType = 84;
				}
				if (num22 == 8)
				{
					dustType = 14;
				}
				if (num22 == 9)
				{
					dustType = 23;
				}
				if (num22 == 10)
				{
					dustType = 25;
				}
				if (num22 == 11)
				{
					dustType = 48;
				}
				if (num22 == 12)
				{
					dustType = 144;
				}
				if (num22 == 13)
				{
					dustType = 49;
				}
				if (num22 == 14)
				{
					dustType = 145;
				}
				if (num22 == 15)
				{
					dustType = 50;
				}
				if (num22 == 16)
				{
					dustType = 146;
				}
				if (num22 == 17)
				{
					dustType = 128;
				}
				if (num22 == 18)
				{
					dustType = 84;
				}
				if (num22 == 19)
				{
					dustType = 117;
				}
				if (num22 == 20)
				{
					dustType = 26;
				}
			}
			if (type == 185)
			{
				if (frameY == 18)
				{
					int num23 = (int)(frameX / 36);
					if (num23 < 6)
					{
						dustType = 1;
					}
					else if (num23 < 16)
					{
						dustType = 26;
					}
					else if (num23 == 16)
					{
						dustType = 9;
					}
					else if (num23 == 17)
					{
						dustType = 11;
					}
					else if (num23 == 18)
					{
						dustType = 10;
					}
					else if (num23 == 19)
					{
						dustType = 86;
					}
					else if (num23 == 20)
					{
						dustType = 87;
					}
					else if (num23 == 21)
					{
						dustType = 88;
					}
					else if (num23 == 22)
					{
						dustType = 89;
					}
					else if (num23 == 23)
					{
						dustType = 90;
					}
					else if (num23 == 24)
					{
						dustType = 91;
					}
					else if (num23 < 31)
					{
						dustType = 80;
					}
					else if (num23 < 33)
					{
						dustType = 7;
					}
					else if (num23 < 34)
					{
						dustType = 8;
					}
					else if (num23 < 39)
					{
						dustType = 30;
					}
					else if (num23 < 42)
					{
						dustType = 1;
					}
				}
				else
				{
					int num24 = (int)(frameX / 18);
					if (num24 < 6)
					{
						dustType = 1;
					}
					else if (num24 < 12)
					{
						dustType = 0;
					}
					else if (num24 < 27)
					{
						dustType = 26;
					}
					else if (num24 < 32)
					{
						dustType = 1;
					}
					else if (num24 < 35)
					{
						dustType = 0;
					}
					else if (num24 < 46)
					{
						dustType = 80;
					}
					else if (num24 < 52)
					{
						dustType = 30;
					}
				}
			}
			if (type == 184)
			{
				int num25 = (int)(frameX / 22);
				dustType = 93 + num25;
			}
			if (type == 237)
			{
				dustType = 148;
			}
			if (type == 157)
			{
				dustType = 77;
			}
			if (type == 158 || type == 232)
			{
				dustType = 78;
			}
			if (type == 159)
			{
				dustType = 78;
			}
			if (type == 15)
			{
				dustType = -1;
			}
			if (type == 191)
			{
				dustType = 7;
			}
			if (type == 5)
			{
				dustType = 7;
			}
			if (type == 323)
			{
				dustType = 215;
			}
			if (type == 137)
			{
				dustType = 1;
				int num29 = (int)(frameY / 18);
				if (num29 > 0)
				{
					dustType = 148;
				}
			}
			if (type == 212)
			{
				dustType = -1;
			}
			if (type == 213)
			{
				dustType = 129;
			}
			if (type == 214)
			{
				dustType = 1;
			}
			if (type == 215)
			{
				dustType = 6;
			}
			if (type == 325)
			{
				dustType = 81;
			}
			if (type == 251)
			{
				dustType = 189;
			}
			if (type == 252)
			{
				dustType = 190;
			}
			if (type == 253)
			{
				dustType = 191;
			}
			if (type == 254)
			{
				if (frameX < 72)
				{
					dustType = 3;
				}
				else if (frameX < 108)
				{
					dustType = 3;
					if (Main.rand.Next(3) == 0)
					{
						dustType = 189;
					}
				}
				else if (frameX < 144)
				{
					dustType = 3;
					if (Main.rand.Next(2) == 0)
					{
						dustType = 189;
					}
				}
				else
				{
					dustType = 3;
					if (Main.rand.Next(4) != 0)
					{
						dustType = 189;
					}
				}
			}
			if (type == 21)
			{
				if (frameX >= 1008)
				{
					dustType = -1;
				}
				else if (frameX >= 612)
				{
					dustType = 11;
				}
				else if (frameX >= 576)
				{
					dustType = 148;
				}
				else if (frameX >= 540)
				{
					dustType = 26;
				}
				else if (frameX >= 504)
				{
					dustType = 126;
				}
				else if (frameX >= 468)
				{
					dustType = 116;
				}
				else if (frameX >= 432)
				{
					dustType = 7;
				}
				else if (frameX >= 396)
				{
					dustType = 11;
				}
				else if (frameX >= 360)
				{
					dustType = 10;
				}
				else if (frameX >= 324)
				{
					dustType = 79;
				}
				else if (frameX >= 288)
				{
					dustType = 78;
				}
				else if (frameX >= 252)
				{
					dustType = 77;
				}
				else if (frameX >= 216)
				{
					dustType = 1;
				}
				else if (frameX >= 180)
				{
					dustType = 7;
				}
				else if (frameX >= 108)
				{
					dustType = 37;
				}
				else if (frameX >= 36)
				{
					dustType = 10;
				}
				else
				{
					dustType = 7;
				}
			}
			if (type == 2)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 0;
				}
				else
				{
					dustType = 2;
				}
			}
			if (Main.tileMoss[(int)type])
			{
				dustType = (int)(type - 179 + 93);
			}
			if (type == 127)
			{
				dustType = 67;
			}
			if (type == 91)
			{
				dustType = -1;
			}
			if (type == 198)
			{
				dustType = 109;
			}
			if (type == 26)
			{
				if (frameX >= 54)
				{
					dustType = 5;
				}
				else
				{
					dustType = 8;
				}
			}
			if (type == 34)
			{
				dustType = -1;
			}
			if (type == 6)
			{
				dustType = 8;
			}
			if (type == 7 || type == 47 || type == 284)
			{
				dustType = 9;
			}
			if (type == 8 || type == 45 || type == 102)
			{
				dustType = 10;
			}
			if (type == 9 || type == 42 || type == 46 || type == 126 || type == 136)
			{
				dustType = 11;
			}
			if (type == 166 || type == 175)
			{
				dustType = 81;
			}
			if (type == 167)
			{
				dustType = 82;
			}
			if (type == 168 || type == 176)
			{
				dustType = 83;
			}
			if (type == 169 || type == 177)
			{
				dustType = 84;
			}
			if (type == 199)
			{
				dustType = 117;
			}
			if (type == 205)
			{
				dustType = 125;
			}
			if (type == 201)
			{
				dustType = 125;
			}
			if (type == 211)
			{
				dustType = 128;
			}
			if (type == 227)
			{
				int num30 = (int)(frameX / 34);
				if (num30 == 0 || num30 == 1)
				{
					dustType = 26;
				}
				else if (num30 == 3)
				{
					dustType = 3;
				}
				else if (num30 == 2 || num30 == 4 || num30 == 5 || num30 == 6)
				{
					dustType = 40;
				}
				else if (num30 == 7)
				{
					dustType = 117;
				}
			}
			if (type == 204)
			{
				dustType = 117;
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 1;
				}
			}
			if (type == 203)
			{
				dustType = 117;
			}
			if (type == 243)
			{
				if (Main.rand.Next(2) == 0)
				{
					dustType = 7;
				}
				else
				{
					dustType = 13;
				}
			}
			if (type == 244)
			{
				if (Main.rand.Next(2) == 0)
				{
					dustType = 1;
				}
				else
				{
					dustType = 13;
				}
			}
			else if ((type >= 275 && type <= 282) || (type == 285 || type == 286 || (type >= 288 && type <= 297)) || (type >= 316 && type <= 318) || type == 298 || type == 299 || type == 309 || type == 310 || type == 339)
			{
				dustType = 13;
				if (Main.rand.Next(3) != 0)
				{
					dustType = -1;
				}
			}
			if (type == 13)
			{
				if (frameX >= 90)
				{
					dustType = -1;
				}
				else
				{
					dustType = 13;
				}
			}
			if (type == 189)
			{
				dustType = 16;
			}
			if (type == 12)
			{
				dustType = 12;
			}
			if (type == 3 || type == 73)
			{
				dustType = 3;
			}
			if (type == 54)
			{
				dustType = 13;
			}
			if (type == 22 || type == 140)
			{
				dustType = 14;
			}
			if (type == 78)
			{
				dustType = 22;
			}
			if (type == 28)
			{
				dustType = 22;
				if (frameY >= 72 && frameY <= 90)
				{
					dustType = 1;
				}
				if (frameY >= 144 && frameY <= 234)
				{
					dustType = 48;
				}
				if (frameY >= 252 && frameY <= 358)
				{
					dustType = 85;
				}
				if (frameY >= 360 && frameY <= 466)
				{
					dustType = 26;
				}
				if (frameY >= 468 && frameY <= 574)
				{
					dustType = 36;
				}
				if (frameY >= 576 && frameY <= 790)
				{
					dustType = 18;
				}
				if (frameY >= 792 && frameY <= 898)
				{
					dustType = 5;
				}
				if (frameY >= 900 && frameY <= 1006)
				{
					dustType = 0;
				}
				if (frameY >= 1008 && frameY <= 1114)
				{
					dustType = 148;
				}
			}
			if (type == 163)
			{
				dustType = 118;
			}
			if (type == 164)
			{
				dustType = 119;
			}
			if (type == 200)
			{
				dustType = 120;
			}
			if (type == 221 || type == 248)
			{
				dustType = 144;
			}
			if (type == 222 || type == 249)
			{
				dustType = 145;
			}
			if (type == 223 || type == 250)
			{
				dustType = 146;
			}
			if (type == 224)
			{
				dustType = 149;
			}
			if (type == 225)
			{
				dustType = 147;
			}
			if (type == 229)
			{
				dustType = 153;
			}
			if (type == 231)
			{
				dustType = 153;
				if (Main.rand.Next(3) == 0)
				{
					dustType = 26;
				}
			}
			if (type == 226)
			{
				dustType = 148;
			}
			if (type == 103)
			{
				dustType = -1;
			}
			if (type == 29)
			{
				dustType = 23;
			}
			if (type == 40)
			{
				dustType = 28;
			}
			if (type == 49)
			{
				dustType = 29;
			}
			if (type == 50)
			{
				dustType = 22;
			}
			if (type == 51)
			{
				dustType = 30;
			}
			if (type == 52)
			{
				dustType = 3;
			}
			if (type == 53 || type == 81 || type == 151 || type == 202 || type == 274)
			{
				dustType = 32;
			}
			if (type == 56 || type == 152)
			{
				dustType = 37;
			}
			if (type == 75)
			{
				dustType = 109;
			}
			if (type == 57 || type == 119 || type == 141 || type == 234)
			{
				dustType = 36;
			}
			if (type == 59 || type == 120)
			{
				dustType = 38;
			}
			if (type == 61 || type == 62 || type == 74 || type == 80 || type == 188 || type == 233 || type == 236)
			{
				dustType = 40;
			}
			if (type == 238)
			{
				if (WorldGen.genRand.Next(3) == 0)
				{
					dustType = 167;
				}
				else
				{
					dustType = 166;
				}
			}
			if (type == 69)
			{
				dustType = 7;
			}
			if (type == 71 || type == 72 || type == 190)
			{
				dustType = 26;
			}
			if (type == 70)
			{
				dustType = 17;
			}
			if (type == 112)
			{
				dustType = 14;
			}
			if (type == 123)
			{
				dustType = 53;
			}
			if (type == 161)
			{
				dustType = 80;
			}
			if (type == 206)
			{
				dustType = 80;
			}
			if (type == 162)
			{
				dustType = 80;
			}
			if (type == 165)
			{
				if (frameX < 54)
				{
					dustType = 80;
				}
				else if (frameX >= 324)
				{
					dustType = 117;
				}
				else if (frameX >= 270)
				{
					dustType = 14;
				}
				else if (frameX >= 216)
				{
					dustType = 1;
				}
				else if (frameX >= 162)
				{
					dustType = 147;
				}
				else if (frameX >= 108)
				{
					dustType = 30;
				}
				else
				{
					dustType = 1;
				}
			}
			if (type == 193)
			{
				dustType = 4;
			}
			if (type == 194)
			{
				dustType = 26;
			}
			if (type == 195)
			{
				dustType = 5;
			}
			if (type == 196)
			{
				dustType = 108;
			}
			if (type == 197)
			{
				dustType = 4;
			}
			if (type == 153)
			{
				dustType = 26;
			}
			if (type == 154)
			{
				dustType = 32;
			}
			if (type == 155)
			{
				dustType = 2;
			}
			if (type == 156)
			{
				dustType = 1;
			}
			if (type == 116 || type == 118 || type == 147 || type == 148)
			{
				dustType = 51;
			}
			if (type == 109)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 0;
				}
				else
				{
					dustType = 47;
				}
			}
			if (type == 110 || type == 113 || type == 115)
			{
				dustType = 47;
			}
			if (type == 107 || type == 121)
			{
				dustType = 48;
			}
			if (type == 108 || type == 122 || type == 146)
			{
				dustType = 49;
			}
			if (type == 111 || type == 145 || type == 150)
			{
				dustType = 50;
			}
			if (type == 133)
			{
				dustType = 50;
				if (frameX >= 54)
				{
					dustType = 146;
				}
			}
			if (type == 134)
			{
				dustType = 49;
				if (frameX >= 36)
				{
					dustType = 145;
				}
			}
			if (type == 149)
			{
				dustType = 49;
			}
			if (AlchemyFlower((int)type))
			{
				int num31 = (int)(frameX / 18);
				if (num31 == 0)
				{
					dustType = 3;
				}
				if (num31 == 1)
				{
					dustType = 3;
				}
				if (num31 == 2)
				{
					dustType = 7;
				}
				if (num31 == 3)
				{
					dustType = 17;
				}
				if (num31 == 4)
				{
					dustType = 3;
				}
				if (num31 == 5)
				{
					dustType = 6;
				}
				if (num31 == 6)
				{
					dustType = 224;
				}
			}
			if (type == 58 || type == 76 || type == 77)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 6;
				}
				else
				{
					dustType = 25;
				}
			}
			if (type == 37)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 6;
				}
				else
				{
					dustType = 23;
				}
			}
			if (type == 32)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 14;
				}
				else
				{
					dustType = 24;
				}
			}
			if (type == 23 || type == 24)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 14;
				}
				else
				{
					dustType = 17;
				}
			}
			if (type == 25 || type == 31)
			{
				if (type == 31 && frameX >= 36)
				{
					dustType = 5;
				}
				else if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 14;
				}
				else
				{
					dustType = 1;
				}
			}
			if (type == 20)
			{
				int num32 = (int)(frameX / 54);
				if (num32 == 1)
				{
					dustType = 122;
				}
				else if (num32 == 2)
				{
					dustType = 78;
				}
				else if (num32 == 3)
				{
					dustType = 77;
				}
				else if (num32 == 4)
				{
					dustType = 121;
				}
				else if (num32 == 5)
				{
					dustType = 79;
				}
				else
				{
					dustType = 7;
				}
			}
			if (type == 27)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					dustType = 3;
				}
				else
				{
					dustType = 19;
				}
			}
			if (type == 129)
			{
				if (frameX == 0 || frameX == 54 || frameX == 108)
				{
					dustType = 68;
				}
				else if (frameX == 18 || frameX == 72 || frameX == 126)
				{
					dustType = 69;
				}
				else
				{
					dustType = 70;
				}
			}
			if (type == 4)
			{
				int num33 = (int)(frameY / 22);
				if (num33 == 0)
				{
					dustType = 6;
				}
				else if (num33 == 8)
				{
					dustType = 75;
				}
				else if (num33 == 9)
				{
					dustType = 135;
				}
				else if (num33 == 10)
				{
					dustType = 158;
				}
				else if (num33 == 11)
				{
					dustType = 169;
				}
				else if (num33 == 12)
				{
					dustType = 156;
				}
				else
				{
					dustType = 58 + num33;
				}
			}
			if (type == 35)
			{
				dustType = 189;
				if (frameX < 36 && WorldGen.genRand.Next(2) == 0)
				{
					dustType = 6;
				}
			}
			if ((type == 34 || type == 42) && Main.rand.Next(2) == 0)
			{
				dustType = 6;
			}
			if (type == 270)
			{
				dustType = -1;
			}
			if (type == 271)
			{
				dustType = -1;
			}
			if (type == 79 || type == 90 || type == 101)
			{
				dustType = -1;
			}
			if (type == 33 || type == 34 || type == 42 || type == 93 || type == 100)
			{
				dustType = -1;
			}
			if (type == 321)
			{
				dustType = 214;
			}
			if (type == 322)
			{
				dustType = 215;
			}
            if (type >= 0 && TileLoader.GetTile(type) != null)
            {  
                dustType = TileLoader.GetTile(type).dustType;
            }
            return dustType;
            #endregion
        }

        /*
         * Returns the minimum pick value required to mine the tile.
         * If the tile at the position is null or inactive, it returns -1.
         */
        public static int GetTileMinPick(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active())
                return -1;
            return GetTileMinPick(tile.type);
        }

        /*
         * Returns the minimum pick value required to mine the tile type.
         */
        public static int GetTileMinPick(int type)
        {
            //TODO: Clean this up...eventually
            #region not gonna be clean folks
            if (type == 211)
            {
                return 200;
            }
            if ((type == 25 || type == 203))
            {
                return 65;
            }
            else if (type == 117)
            {
                return 65;
            }
            else if (type == 37)
            {
                return 50;
            }
            else if (type == 404)
            {
                return 65;
            }
            else if ((type == 22 || type == 204))
            {
                return 55;
            }
            else if (type == 56)
            {
                return 65;
            }
            else if (type == 58)
            {
                return 65;
            }
            else if ((type == 226 || type == 237))
            {
                return 210;
            }
            else if (Main.tileDungeon[type])
            {
                return 65;
            }
            else if (type == 107)
            {
                return 100;
            }
            else if (type == 108)
            {
                return 110;
            }
            else if (type == 111)
            {
                return 150;
            }
            else if (type == 221)
            {
                return 100;
            }
            else if (type == 222)
            {
                return 110;
            }
            else if (type == 223)
            {
                return 150;
            }else
            {
			    ModTile modTile = TileLoader.GetTile(type);
                if (modTile != null) return modTile.minPick;
            }
            return 0;
            #endregion
        }

        public static int GetTileResist(int x, int y, int pickPower)
        {
            Tile tile = Main.tile[x, y];
            int type = tile.type;
            int tileResist = 0;
            if (Main.tileNoFail[type])
            {
                tileResist = 100;
            }
            if (Main.tileDungeon[type] || type == 25 || type == 58 || type == 117 || type == 203)
            {
                tileResist += pickPower / 2;
            }
            else if (type == 48 || type == 232)
            {
                tileResist += pickPower / 4;
            }
            else if (type == 226)
            {
                tileResist += pickPower / 4;
            }
            else if (type == 107 || type == 221)
            {
                tileResist += pickPower / 2;
            }
            else if (type == 108 || type == 222)
            {
                tileResist += pickPower / 3;
            }
            else if (type == 111 || type == 223)
            {
                tileResist += pickPower / 4;
            }
            else if (type == 211)
            {
                tileResist += pickPower / 5;
            }else
            {
                TileLoader.MineDamage(pickPower, ref tileResist);
            }
            int minPick = GetTileMinPick(type);
            if (minPick > pickPower) tileResist = 0;
            if (type == 147 || type == 0 || type == 40 || type == 53 || type == 57 || type == 59 || type == 123 || type == 224 || type == 397)
            {
                tileResist += pickPower;
            }
            if (type == 165 || Main.tileRope[type] || type == 199 || Main.tileMoss[type])
            {
                tileResist = 100;
            }
            if (type == 128 || type == 269)
            {
                if (tile.frameX == 18 || tile.frameX == 54)
                {
                    x--;
                    tile = Main.tile[x, y]; type = tile.type;
                }
                if (tile.frameX >= 100)
                {
                    tileResist = 0;
                }
            }
            if (type == 334)
            {
                if (tile.frameY == 0)
                {
                    y++;
                    tile = Main.tile[x, y]; type = tile.type;
                }
                if (tile.frameY == 36)
                {
                    y--;
                    tile = Main.tile[x, y]; type = tile.type;
                }
                int i = (int)tile.frameX;
                bool frameXOver5000 = i >= 5000;
                bool cannotBreak = false;
                if (!frameXOver5000)
                {
                    int num2 = i / 18;
                    num2 %= 3;
                    x -= num2;
                    tile = Main.tile[x, y]; type = tile.type;
                    if (tile.frameX >= 5000) frameXOver5000 = true;
                }
                if (frameXOver5000)
                {
                    i = (int)tile.frameX;
                    int num3 = 0;
                    while (i >= 5000)
                    {
                        i -= 5000;
                        num3++;
                    }
                    if (num3 != 0) cannotBreak = true;
                }
                if (cannotBreak) tileResist = 0;
            }
            if (!WorldGen.CanKillTile(x, y))
            {
                tileResist = 0;
            }
            return tileResist;
        }

         /*
          * Spawns an explosion at the given position with the given intensity.
          * 
          * explosionIntensity: The radius in all directions from the position of the explosion. (ex. 3 is Bomb, 7 is Dynamite, 10 is Explosives)
          * damage : how much damage to do to npcs/players within the radius. (is not used if noDamage == true)
          * doeffects : True to spawn explosion dust and smoke.
          * dotiles : True to destroy tiles.
          * dodamage : True to damage NPCs/Player.
          */
        public static void SpawnExplosion(Vector2 position, int explosionIntensity = 3, int damage = 50, bool doeffects = true, bool dotiles = true, bool dodamage = true, bool sync = true)
        {
            if(dodamage)
            {
                int[] npcs = BaseAI.GetNPCs(position, -1, (explosionIntensity * 16) + 10f);
                int[] players = BaseAI.GetPlayers(position, (explosionIntensity * 16) + 10f);
                for (int m = 0; m < npcs.Length; m++)
                {
                    NPC npc = Main.npc[npcs[m]];
                    if(npc.dontTakeDamage){ continue; }
                    BaseAI.DamageNPC(npc, Math.Max(1, damage + Main.rand.Next(-7, 8)), Math.Min(10f, explosionIntensity / 3f), null);
                }
                for (int m = 0; m < players.Length; m++)
                {
                    BaseAI.DamagePlayer(Main.player[players[m]], Math.Max(1, damage + Main.rand.Next(-7, 8)), Math.Min(10f, explosionIntensity / 3f), null);
                }
                if (!doeffects && !dotiles) { return; }
            }
            int radiusLeft = (int)(position.X / 16f - (float)explosionIntensity);
            int radiusRight = (int)(position.X / 16f + (float)explosionIntensity);
            int radiusUp = (int)(position.Y / 16f - (float)explosionIntensity);
            int radiusDown = (int)(position.Y / 16f + (float)explosionIntensity);
            if (radiusLeft < 0) { radiusLeft = 0; } if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
            if (radiusUp < 0) { radiusUp = 0; } if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

            if (doeffects)
            {
                BaseAI.SpawnSmoke(position - new Vector2((explosionIntensity * 16), (explosionIntensity * 16)), explosionIntensity * 32, explosionIntensity * 32, explosionIntensity > 4 ? explosionIntensity : 2, 0.75f);
                UnifiedRandom rand = Main.rand;
                for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
                {
                    for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                    {
                        float distX = Math.Abs((float)x1 - position.X / 16f);
                        float distY = Math.Abs((float)y1 - position.Y / 16f);
                        double dist = Math.Sqrt((double)(distX * distX + distY * distY));
                        if (dist < (double)explosionIntensity)
                        {
                            Vector2 dustPos = new Vector2(x1 * 16, y1 * 16);
                            Dust.NewDust(dustPos, 1 + rand.Next(16), 1 + rand.Next(16), 31, 0f, 0f, 100, Color.White, 2f);

                            int fireID = Dust.NewDust(dustPos, 1 + rand.Next(16), 1 + rand.Next(16), 6, 0f, 0f, 100, Color.White, 2f);
                            Main.dust[fireID].noGravity = true;
                            Main.dust[fireID].velocity *= 5f;

                            fireID = Dust.NewDust(dustPos, 1 + rand.Next(16), 1 + rand.Next(16), 6, 0f, 0f, 100, Color.White, 1f);
                            Main.dust[fireID].velocity *= 3f;
                        }
                    }
                }
                if(!dotiles){ return; }
            }
            if (dotiles)
            {
                bool updateWalls = false;
                for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
                {
                    for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                    {
                        float distX = Math.Abs((float)x1 - position.X / 16f);
                        float distY = Math.Abs((float)y1 - position.Y / 16f);
                        double dist = Math.Sqrt((double)(distX * distX + distY * distY));
                        if (dist < (double)explosionIntensity && Main.tile[x1, y1] != null && Main.tile[x1, y1].wall == 0)
                        {
                            updateWalls = true;
                            break;
                        }
                    }
                }
                for (int x2 = radiusLeft; x2 <= radiusRight; x2++)
                {
                    for (int y2 = radiusUp; y2 <= radiusDown; y2++)
                    {
                        float distX = Math.Abs((float)x2 - position.X / 16f);
                        float distY = Math.Abs((float)y2 - position.Y / 16f);
                        double dist = Math.Sqrt((double)(distX * distX + distY * distY));
                        if (dist < (double)explosionIntensity)
                        {
                            bool canExplode = true;
                            if (Main.tile[x2, y2] != null && Main.tile[x2, y2].active())
                            {
                                if (BaseUtility.InArray(BaseConstants.TILEIDS_DUNGEONSTRICT, (int)Main.tile[x2, y2].type) || Main.tile[x2, y2].type == 21 || Main.tile[x2, y2].type == 26 || Main.tile[x2, y2].type == 107 || Main.tile[x2, y2].type == 108 || Main.tile[x2, y2].type == 111)
                                {
                                    canExplode = false;
                                }
                                if (!Main.hardMode && Main.tile[x2, y2].type == 58) { canExplode = false; }

                                if (canExplode)
                                {
                                    WorldGen.KillTile(x2, y2, false, false, false);
                                    if (sync && !Main.tile[x2, y2].active() && Main.netMode != 0) { NetMessage.SendData(BaseConstants.NET_TILE_UPDATE, -1, -1, NetworkText.FromLiteral(""), 0, (float)x2, (float)y2, 0f, 0); }
                                }
                            }
                            if (canExplode)
                            {
                                for (int x3 = x2 - 1; x3 <= x2 + 1; x3++)
                                {
                                    for (int y3 = y2 - 1; y3 <= y2 + 1; y3++)
                                    {
                                        if (Main.tile[x3, y3] != null && Main.tile[x3, y3].wall > 0 && updateWalls)
                                        {
                                            WorldGen.KillWall(x3, y3, false);
                                            if(sync && Main.tile[x3, y3].wall == 0 && Main.netMode != 0) { NetMessage.SendData(BaseConstants.NET_TILE_UPDATE, -1, -1, NetworkText.FromLiteral(""), 2, (float)x3, (float)y3, 0f, 0); }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

       /*
        * Plays the tile at (tileX, tileY)'s hit sound.
        */       
        public static void PlayTileHitSound(int tileX, int tileY)
        {
            Tile tile = Main.tile[tileX, tileY];
            if(tile != null)
            {
                PlayTileHitSound(tileX * 16, tileY * 16, tile.type);
            }
        }

        /*
         * Plays a specific tile type's hit sound at the given position.
         */
        public static void PlayTileHitSound(float x, float y, int tileType)
        {
			//TODO: FIX
            /*if (TileDef.sound.Length < tileType && TileDef.sound[tileType] > 0)
            {
				int hitSound = TileDef.sound[tileType];
                int list = 0;
				list = TileDef.soundGroup[tileType];
                Main.PlaySound(list, (int)x, (int)y, hitSound);
            }*/
            if (tileType >= 0 && TileLoader.GetTile(tileType) != null)
            {
                ModTile tile = TileLoader.GetTile(tileType);
                Main.PlaySound(tile.soundStyle, (int)x, (int)y, tile.soundType);
            }
            else if (tileType == 127)
                Main.PlaySound(2, (int)x, (int)y, 27);
            else if (AlchemyFlower((int)tileType) || tileType == 3 || tileType == 110 || tileType == 24 || tileType == 32 || tileType == 51 || tileType == 52 || tileType == 61 || tileType == 62 || tileType == 69 || tileType == 71 || tileType == 73 || tileType == 74 || tileType == 113 || tileType == 115)
                Main.PlaySound(6, (int)x, (int)y, 1);
            else if (tileType == 1 || tileType == 6 || tileType == 7 || tileType == 8 || tileType == 9 || tileType == 22 || tileType == 140 || tileType == 25 || tileType == 37 || tileType == 38 || tileType == 39 || tileType == 41 || tileType == 43 || tileType == 44 || tileType == 45 || tileType == 46 || tileType == 47 || tileType == 48 || tileType == 56 || tileType == 58 || tileType == 63 || tileType == 64 || tileType == 65 || tileType == 66 || tileType == 67 || tileType == 68 || tileType == 75 || tileType == 76 || tileType == 107 || tileType == 108 || tileType == 111 || tileType == 117 || tileType == 118 || tileType == 119 || tileType == 120 || tileType == 121 || tileType == 122)
                Main.PlaySound(21, (int)x, (int)y, 1);
            else if (tileType != 138)
                Main.PlaySound(0, (int)x, (int)y, 1);
        }

        /*
         * Goes through a square area given by the x, y and width, height params, and returns true if they are all of the type given.
         */
        public static bool IsType(int x, int y, int width, int height, int type)
        {
            for(int x1 = x; x1 < x + width; x1++)
                for (int y1 = y; y1 < y + height; y1++)
                {
                    Tile tile = Main.tile[x1, y1];
                    if(tile == null || !tile.active() || tile.type != type)
                    {
                        return false;
                    }
                }
            return true;
        }

		#region probably depecrated
        /*
         * Convenience method for CheckTilePlacement. The Y coordinate passed in is the topmost Y not the bottommost Y.
         *
        public static bool CheckTile(int x, int y, int width, int height, int type)
        {
			return true; // Config.CheckTilePlacement(x, y + height - 1, width, height, type);
        }

        /*
         * Similar to CheckTilePlacement but only requires one tile to fufill requirements instead of the entire side of the tile.
         * (For tiles > 1 x 1 in size)
         * The Y coordinate passed in is the topmost Y not the bottommost Y.
         *
        public static bool CheckTileOneOnly(int x, int y, int width, int height, int type)
        {
            string places = "solid ";
            if(Main.tileSolid[type]) places = "solid wall ceiling side ";
            if(TileDef.placementConditions.Length < type)
            {
                places = TileDef.placementConditions[type];
            }
            return CheckTileOneOnly(x, y, width, height, type, places.Contains("solid "), places.Contains("nonsolid "), places.Contains("solidTop "), places.Contains("ceiling "), places.Contains("wall "), places.Contains("side "));
        }

        /*
         * Similar to CheckTilePlacement but only requires one tile to fufill requirements instead of the entire side of the tile.
         * (For tiles > 1 x 1 in size)
         * The Y coordinate passed in is also the topmost Y not the bottommost Y.
         *
        public static bool CheckTileOneOnly(int x, int y, int width, int height, int type, bool solid, bool nonsolid, bool solidTop, bool ceiling, bool wall, bool side)
        {
            int startX = x;
            int startY = y;
            int endX = x + width - 1;
            int endY = y + height - 1;
            if(solid)
            {
                for(int i = startX; i <= endX; i++)
                {
                    if ((Main.tile[i, endY + 1] != null && Main.tile[i, endY + 1].active() && Main.tileSolid[Main.tile[i, endY + 1].type]))
                    {
                        return true;
                    }
                }
            }
            if(nonsolid)
            {
                for (int i = startX; i <= endX; i++)
                {
                    if ((Main.tile[i, endY + 1] != null && Main.tile[i, endY + 1].active() && !Main.tileSolid[Main.tile[i, endY + 1].type]))
                    {
                        return true;
                    }
                }
            }
            if(solidTop)
            {
                for (int i = startX; i <= endX; i++)
                {
                    if ((Main.tile[i, endY + 1] != null && Main.tile[i, endY + 1].active() && Main.tileSolidTop[Main.tile[i, endY + 1].type]))
                    {
                        return true;
                    }
                }
            }
            if(ceiling)
            {
                for (int i = startX; i <= endX; i++)
                {
                    if((Main.tile[i, startY - 1] != null && Main.tile[i, startY - 1].active() && Main.tileSolid[Main.tile[i, startY - 1].type]))
                    {
                        return true;
                    }
                }
            }
            if(wall)
            {
                for(int i = startX; i <= endX; i++)
                {
                    for(int j = startY; j <= endY; j++)
                    {
                        if((Main.tile[i, j] != null && Main.tile[i, j].wall > 0))
                        {
                            return true;
                        }
                    }
                }
            }
            if(side)
            {
                for(int j = startY; j <= endY; j++)
                {
                    if((Main.tile[startX - 1, j] != null && Main.tile[startX - 1, j].active() && Main.tileSolid[Main.tile[startX - 1, startY].type]) || (Main.tile[endX + 1, j] != null && Main.tile[endX + 1, j].active() && Main.tileSolid[Main.tile[endX + 1, startY].type]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        */
        #endregion
        
        /**
         * Return the count of tiles and walls of the given types within the given distance.
         * If a location has a tile and a wall it is only counted once.
         */
        public static int GetTileAndWallCount(Vector2 tileCenter, int[] tileTypes, int[] wallTypes, int distance = 35)
        {
            int tileCount = 0;
            bool addedTile = false;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null) { continue; }
                    addedTile = false;
                    if (tile.active())
                    {
                        foreach (int i in tileTypes)
                        {
                            if (tile.type == i) { tileCount++; addedTile = true; break; }
                        }
                    }
                    if (!addedTile)
                    {
                        foreach (int i in wallTypes)
                        {
                            if (tile.wall == i) { tileCount++; break; }
                        }
                    }
                    addedTile = false;
                }
            }
            return tileCount;
        }

        /**
         * Return the count of walls of the given types within the given distance.
         */
        public static int GetWallCount(Vector2 tileCenter, int[] wallTypes, int distance = 35)
        {
            int wallCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null) { continue; }
                    foreach (int i in wallTypes)
                    {
                        if (tile.wall == i) { wallCount++; break; }
                    }
                }
            }
            return wallCount;
        }

        /**
         * Return the count of tiles of the given types within the given distance.
         */
        public static int GetTileCount(Vector2 tileCenter, int[] tileTypes, int distance = 35)
        {
            int tileCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null || !tile.active()) { continue; }
                    foreach (int i in tileTypes)
                    {
                        if (tile.type == i) { tileCount++; break; }
                    }
                }
            }
            return tileCount;
        }
    }
}