
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using System.Linq;
using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Blocks;

namespace Macrocosm.Common.TileFrame
{
	public class TileFraming 
	{
		#region Framing methods
		// Framing legend:
		// ---------------
		//  The tile   | T	
		//  Solid      | #	
		//  Empty      | _
		//  Don't care |  
		// ---------------

		/// <summary> Style currently used for plating blocks with special frames (e.g. Moon Base Plating) </summary>
		public static bool PlatingStyle(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			var info = new TileNeighbourInfo(i, j).TypedSolid(tile.TileType);
			bool shouldRunBaseFraming = true;

			void SetFrame(int x, int y)
			{
				tile.TileFrameX = (short)x;
				tile.TileFrameY = (short)y;
				shouldRunBaseFraming = false;
			}

			//   _  
			// _ T #
			//   # _
			if (info.Right && info.Bottom && !info.Top && !info.Left && !info.BottomRight)
				SetFrame(234, 0);

			// _ #  
			// _ T #
			//   _  
			if (info.Top && info.Right && !info.Bottom && !info.Left && !info.TopRight)
				SetFrame(234, 36);

			//   _  
			// # T _
			// _ #  
			if (info.Left && info.Bottom && !info.Top && !info.Right && !info.BottomLeft)
				SetFrame(270, 0);

			// _ #  
			// # T _
			//   _  
			if (info.Top && info.Left && !info.Bottom && !info.Right && !info.TopLeft)
				SetFrame(270, 36);


			//   # _
			// _ T #
			//   # _
			if (info.Top && info.Bottom & info.Right && !info.Left && !info.TopRight && !info.BottomRight)
				SetFrame(234, 18);

			// _ #  
			// # T _
			// _ #  
			if (info.Top && info.Bottom & info.Left && !info.Right && !info.TopLeft && !info.BottomLeft)
				SetFrame(270, 18);

			// _ # _
			// # T #
			//   _  
			if (info.Left && info.Right && info.Top && !info.Bottom && !info.TopLeft && !info.TopRight)
				SetFrame(252, 36);

			//   _  
			// # T #
			// _ # _
			if (info.Left && info.Right && info.Bottom && !info.Top && !info.BottomRight && !info.BottomLeft)
				SetFrame(252, 0);

			//   # _
			// _ T #
			//   # #
			if (info.Bottom && info.BottomRight && info.Right && !info.TopRight && info.Top && !info.Left)
				SetFrame(288, 0);

			//   # #
			// _ T #
			//   # _
			if (info.Top && info.TopRight && info.Right && !info.BottomRight && info.Bottom && !info.Left)
				SetFrame(288, 18);

			// _ #  
			// # T _
			// # #  
			if (info.Bottom && info.BottomLeft && info.Left && !info.TopLeft && info.Top && !info.Right)
				SetFrame(306, 0);

			// # #  
			// # T _
			// _ #  
			if (info.Top && info.TopLeft && info.Left && !info.BottomLeft && info.Bottom && !info.Right)
				SetFrame(306, 18);

			//   _   
			// # T # 
			// _ # # 
			if (info.Right && info.BottomRight && info.Bottom && !info.BottomLeft && info.Left && !info.Top)
				SetFrame(288, 36);

			//   _   
			// # T # 
			// # # _ 
			if (info.Left && info.BottomLeft && info.Bottom && !info.BottomRight && info.Right && !info.Top)
				SetFrame(306, 36);

			// _ # # 
			// # T #
			//   _   
			if (info.Right && info.TopRight && info.Top && !info.TopLeft && info.Left && !info.Bottom)
				SetFrame(288, 54);

			// # # _ 
			// # T # 
			// _ _   
			if (info.Left && info.TopLeft && info.Top && !info.TopRight && info.Right && !info.Bottom)
				SetFrame(306, 54);

			// Neighbour count dependent frames

			switch (info.Count)
			{
				case 4:
					{
						// _ # _
						// # T #
						// _ # _
						if (info.Top && info.Right && info.Bottom && info.Left)
							SetFrame(252, 18);

						break;
					}

				case 5:
					{
						// _ # _
						// # T #
						// _ # #
						if (!info.TopLeft && !info.TopRight && !info.BottomLeft)
							SetFrame(216, 54);

						// _ # _
						// # T #
						// # # _
						if (!info.TopLeft && !info.TopRight && !info.BottomRight)
							SetFrame(234, 54);

						// _ # #
						// # T #
						// _ # _
						if (!info.BottomLeft && !info.BottomRight && !info.TopLeft)
							SetFrame(216, 72);

						// # # _
						// # T #
						// _ # _
						if (!info.BottomLeft && !info.BottomRight && !info.TopRight)
							SetFrame(234, 72);

						break;
					}

				case 6:
					{
						// _ # #
						// # T #
						// # # _
						if (!info.TopLeft && !info.BottomRight)
							SetFrame(306, 72);

						// # # _
						// # T #
						// _ # #
						if (!info.TopRight && !info.BottomLeft)
							SetFrame(288, 72);

						break;
					}

				case 7:
					{
						// # # _
						// # T #
						// # # #
						if (!info.TopRight)
							SetFrame(270, 54);

						// _ # #
						// # T #
						// # # #
						if (!info.TopLeft)
							SetFrame(252, 54);

						// # # #
						// # T #
						// # # _
						if (!info.BottomRight)
							SetFrame(270, 72);

						// # # #
						// # T #
						// _ # #
						if (!info.BottomLeft)
							SetFrame(252, 72);

						break;
					}
			}

			if (shouldRunBaseFraming)
				BasicFraming(i, j);

			return false;
			//return shouldRunBaseFraming;
		}

		/// <summary> Copy of the common framing all blocks in Terraria use </summary>
		public static void BasicFraming(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int type = tile.TileType;

			Tile tileUp = Main.tile[i, j - 1];
			Tile tileUpRight = Main.tile[i + 1, j - 1];
			Tile tileRight = Main.tile[i + 1, j];
			Tile tileDownRight = Main.tile[i + 1, j + 1];
			Tile tileDown = Main.tile[i, j + 1];
			Tile tileDownLeft = Main.tile[i - 1, j + 1];
			Tile tileLeft = Main.tile[i - 1, j];
			Tile tileUpLeft = Main.tile[i - 1, j - 1];

			int up = tileUp.HasTile ? tileUp.TileType : -1;
			int upRight = tileUpRight.HasTile ? tileUpRight.TileType : -1;
			int right = tileRight.HasTile ? tileRight.TileType : -1;
			int downRight = tileDownRight.HasTile ? tileDownRight.TileType : -1;
			int down = tileDown.HasTile ? tileDown.TileType : -1;
			int downLeft = tileDownLeft.HasTile ? tileDownLeft.TileType : -1;
			int left = tileLeft.HasTile ? tileLeft.TileType : -1;
			int upLeft = tileUpLeft.HasTile ? tileUpLeft.TileType : -1;

			Point frame = new(-1, -1);

			int variation = WorldGen.genRand.Next(3);

			if (frame.X < 0 || frame.Y < 0)
			{
				if (up == type && down == type && left == type && right == type)
				{
					if (upLeft != type && upRight != type)
					{
						switch (variation)
						{
							case 0:
								frame.X = 108;
								frame.Y = 18;
								break;
							case 1:
								frame.X = 126;
								frame.Y = 18;
								break;
							default:
								frame.X = 144;
								frame.Y = 18;
								break;
						}
					}
					else if (downLeft != type && downRight != type)
					{
						switch (variation)
						{
							case 0:
								frame.X = 108;
								frame.Y = 36;
								break;
							case 1:
								frame.X = 126;
								frame.Y = 36;
								break;
							default:
								frame.X = 144;
								frame.Y = 36;
								break;
						}
					}
					else if (upLeft != type && downLeft != type)
					{
						switch (variation)
						{
							case 0:
								frame.X = 180;
								frame.Y = 0;
								break;
							case 1:
								frame.X = 180;
								frame.Y = 18;
								break;
							default:
								frame.X = 180;
								frame.Y = 36;
								break;
						}
					}
					else if (upRight != type && downRight != type)
					{
						switch (variation)
						{
							case 0:
								frame.X = 198;
								frame.Y = 0;
								break;
							case 1:
								frame.X = 198;
								frame.Y = 18;
								break;
							default:
								frame.X = 198;
								frame.Y = 36;
								break;
						}
					}
					else
					{
						switch (variation)
						{
							case 0:
								frame.X = 18;
								frame.Y = 18;
								break;
							case 1:
								frame.X = 36;
								frame.Y = 18;
								break;
							default:
								frame.X = 54;
								frame.Y = 18;
								break;
						}
					}
				}
				else if (up != type && down == type && left == type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 18;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 36;
							frame.Y = 0;
							break;
						default:
							frame.X = 54;
							frame.Y = 0;
							break;
					}
				}
				else if (up == type && down != type && left == type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 18;
							frame.Y = 36;
							break;
						case 1:
							frame.X = 36;
							frame.Y = 36;
							break;
						default:
							frame.X = 54;
							frame.Y = 36;
							break;
					}
				}
				else if (up == type && down == type && left != type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 0;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 0;
							frame.Y = 18;
							break;
						default:
							frame.X = 0;
							frame.Y = 36;
							break;
					}
				}
				else if (up == type && down == type && left == type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 72;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 72;
							frame.Y = 18;
							break;
						default:
							frame.X = 72;
							frame.Y = 36;
							break;
					}
				}
				else if (up != type && down == type && left != type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 0;
							frame.Y = 54;
							break;
						case 1:
							frame.X = 36;
							frame.Y = 54;
							break;
						default:
							frame.X = 72;
							frame.Y = 54;
							break;
					}
				}
				else if (up != type && down == type && left == type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 18;
							frame.Y = 54;
							break;
						case 1:
							frame.X = 54;
							frame.Y = 54;
							break;
						default:
							frame.X = 90;
							frame.Y = 54;
							break;
					}
				}
				else if (up == type && down != type && left != type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 0;
							frame.Y = 72;
							break;
						case 1:
							frame.X = 36;
							frame.Y = 72;
							break;
						default:
							frame.X = 72;
							frame.Y = 72;
							break;
					}
				}
				else if (up == type && down != type && left == type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 18;
							frame.Y = 72;
							break;
						case 1:
							frame.X = 54;
							frame.Y = 72;
							break;
						default:
							frame.X = 90;
							frame.Y = 72;
							break;
					}
				}
				else if (up == type && down == type && left != type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 90;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 90;
							frame.Y = 18;
							break;
						default:
							frame.X = 90;
							frame.Y = 36;
							break;
					}
				}
				else if (up != type && down != type && left == type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = 72;
							break;
						case 1:
							frame.X = 126;
							frame.Y = 72;
							break;
						default:
							frame.X = 144;
							frame.Y = 72;
							break;
					}
				}
				else if (up != type && down == type && left != type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 126;
							frame.Y = 0;
							break;
						default:
							frame.X = 144;
							frame.Y = 0;
							break;
					}
				}
				else if (up == type && down != type && left != type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = 54;
							break;
						case 1:
							frame.X = 126;
							frame.Y = 54;
							break;
						default:
							frame.X = 144;
							frame.Y = 54;
							break;
					}
				}
				else if (up != type && down != type && left != type && right == type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 162;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 162;
							frame.Y = 18;
							break;
						default:
							frame.X = 162;
							frame.Y = 36;
							break;
					}
				}
				else if (up != type && down != type && left == type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 216;
							frame.Y = 0;
							break;
						case 1:
							frame.X = 216;
							frame.Y = 18;
							break;
						default:
							frame.X = 216;
							frame.Y = 36;
							break;
					}
				}
				else if (up != type && down != type && left != type && right != type)
				{
					switch (variation)
					{
						case 0:
							frame.X = 162;
							frame.Y = 54;
							break;
						case 1:
							frame.X = 180;
							frame.Y = 54;
							break;
						default:
							frame.X = 198;
							frame.Y = 54;
							break;
					}
				}
			}

			if (frame.X <= -1 || frame.Y <= -1)
			{
				switch (variation)
				{
					case 0:
						frame.X = 18;
						frame.Y = 18;
						break;
					case 1:
						frame.X = 36;
						frame.Y = 18;
						break;
					default:
						frame.X = 54;
						frame.Y = 18;
						break;
				}
			}

			Main.tile[i, j].TileFrameX = (short)frame.X;
			Main.tile[i, j].TileFrameY = (short)frame.Y;
		}

		/// <summary>
		/// Appears to be broken, do not use 
		/// Adaption of the vanilla tile blend code using dedicated frames
		/// </summary>
		public static void BlendLikeDirt(int i, int j, int typeToBlendWith, bool asDirt = false)
			=> BlendLikeDirt(i, j, new int[] { typeToBlendWith }, asDirt);

		/// <summary>
		/// Appears to be broken, do not use 
		/// Adaption of the vanilla tile blend code using dedicated frames
		/// </summary>
		public static void BlendLikeDirt(int i, int j, int[] typesToBlendWith, bool asDirt = false)
		{
			Tile tile = Main.tile[i, j];
			int type = tile.TileType;

			Tile tileUp = Main.tile[i, j - 1];
			Tile tileUpRight = Main.tile[i + 1, j - 1];
			Tile tileRight = Main.tile[i + 1, j];
			Tile tileDownRight = Main.tile[i + 1, j + 1];
			Tile tileDown = Main.tile[i, j + 1];
			Tile tileDownLeft = Main.tile[i - 1, j + 1];
			Tile tileLeft = Main.tile[i - 1, j];
			Tile tileUpLeft = Main.tile[i - 1, j - 1];

			int up = tileUp.HasTile ? tileUp.TileType : -1;
			int upRight = tileUpRight.HasTile ? tileUpRight.TileType : -1;
			int right = tileRight.HasTile ? tileRight.TileType : -1;
			int downRight = tileDownRight.HasTile ? tileDownRight.TileType : -1;
			int down = tileDown.HasTile ? tileDown.TileType : -1;
			int downLeft = tileDownLeft.HasTile ? tileDownLeft.TileType : -1;
			int left = tileLeft.HasTile ? tileLeft.TileType : -1;
			int upLeft = tileUpLeft.HasTile ? tileUpLeft.TileType : -1;

			bool blendUp = typesToBlendWith.Contains(up);
			bool blendDown = typesToBlendWith.Contains(down);
			bool blendLeft = typesToBlendWith.Contains(left);
			bool blendRight = typesToBlendWith.Contains(right);

			Point frame = new(-1, -1);

			int variation = WorldGen.genRand.Next(3);

			#region Ignore other blocks

			if (up > -1 && up != type && !blendUp)
				up = -1;

			if (down > -1 && down != type && !blendDown)
				down = -1;

			if (left > -1 && left != type && !blendLeft)
				left = -1;

			if (right > -1 && right != type && !blendRight)
				right = -1;

			#endregion

			if (asDirt)
			{
				if (blendUp)
				{
					WorldGen.TileFrame(i, j - 1);
					up = Utility.HasBlendingFrame(i, j - 1) ? type : -1;
				}
				if (blendDown)
				{
					WorldGen.TileFrame(i, j + 1);
					down = Utility.HasBlendingFrame(i, j + 1) ? type : -1;
				}
				if (blendLeft)
				{
					WorldGen.TileFrame(i - 1, j);
					left = Utility.HasBlendingFrame(i - 1, j) ? type : -1;
				}
				if (blendRight)
				{
					WorldGen.TileFrame(i + 1, j);
					right = Utility.HasBlendingFrame(i + 1, j) ? type : -1;
				}
			}

			#region Ignore unconnected slopes

			if (tileUp.Slope == SlopeType.SlopeUpLeft || tileUp.Slope == SlopeType.SlopeUpRight)
				up = -1;

			if (tileRight.Slope == SlopeType.SlopeDownRight || tileRight.Slope == SlopeType.SlopeUpRight)
				right = -1;

			if (tileDown.Slope == SlopeType.SlopeDownLeft || tileDown.Slope == SlopeType.SlopeDownRight)
				down = -1;

			if (tileLeft.Slope == SlopeType.SlopeDownLeft || tileLeft.Slope == SlopeType.SlopeUpLeft)
				left = -1;

			#endregion

			int frameOffsetY;

			#region Blending
			if (up != -1 && down != -1 && left != -1 && right != -1)
			{
				if (blendUp && down == type && left == type && right == type)
				{
					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 144;
							frame.Y = frameOffsetY + 108;
							break;
						case 1:
							frame.X = 162;
							frame.Y = frameOffsetY + 108;
							break;
						default:
							frame.X = 180;
							frame.Y = frameOffsetY + 108;
							break;
					}

					//mergeUp = true;
				}
				else if (up == type && blendDown && left == type && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 144;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 162;
							frame.Y = frameOffsetY + 90;
							break;
						default:
							frame.X = 180;
							frame.Y = frameOffsetY + 90;
							break;
					}

					//mergeDown = true;
				}
				else if (up == type && down == type && blendLeft && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 162;
							frame.Y = frameOffsetY + 126;
							break;
						case 1:
							frame.X = 162;
							frame.Y = frameOffsetY + 144;
							break;
						default:
							frame.X = 162;
							frame.Y = frameOffsetY + 162;
							break;
					}

					//mergeLeft = true;
				}
				else if (up == type && down == type && left == type && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 144;
							frame.Y = frameOffsetY + 126;
							break;
						case 1:
							frame.X = 144;
							frame.Y = frameOffsetY + 144;
							break;
						default:
							frame.X = 144;
							frame.Y = frameOffsetY + 162;
							break;
					}

					//mergeRight = true;
				}
				else if (blendUp && down == type && blendLeft && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 36;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 36;
							frame.Y = frameOffsetY + 126;
							break;
						default:
							frame.X = 36;
							frame.Y = frameOffsetY + 162;
							break;
					}

					//mergeUp = true;
					//mergeLeft = true;
				}
				else if (blendUp && down == type && left == type && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 54;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 54;
							frame.Y = frameOffsetY + 126;
							break;
						default:
							frame.X = 54;
							frame.Y = frameOffsetY + 162;
							break;
					}

					//mergeUp = true;
					//mergeRight = true;
				}
				else if (up == type && blendDown && blendLeft && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 36;
							frame.Y = frameOffsetY + 108;
							break;
						case 1:
							frame.X = 36;
							frame.Y = frameOffsetY + 144;
							break;
						default:
							frame.X = 36;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeDown = true;
					//mergeLeft = true;
				}
				else if (up == type && blendDown && left == type && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;


					switch (variation)
					{
						case 0:
							frame.X = 54;
							frame.Y = frameOffsetY + 108;
							break;
						case 1:
							frame.X = 54;
							frame.Y = frameOffsetY + 144;
							break;
						default:
							frame.X = 54;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeDown = true;
					//mergeRight = true;
				}
				else if (up == type && down == type && blendLeft && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 180;
							frame.Y = frameOffsetY + 126;
							break;
						case 1:
							frame.X = 180;
							frame.Y = frameOffsetY + 144;
							break;
						default:
							frame.X = 180;
							frame.Y = frameOffsetY + 162;
							break;
					}

					//mergeLeft = true;
					//mergeRight = true;
				}
				else if (blendUp && blendDown && left == type && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 144;
							frame.Y = frameOffsetY + 180;
							break;
						case 1:
							frame.X = 162;
							frame.Y = frameOffsetY + 180;
							break;
						default:
							frame.X = 180;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeUp = true;
					//mergeDown = true;
				}
				else if (blendUp && down == type && blendLeft && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 198;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 198;
							frame.Y = frameOffsetY + 108;
							break;
						default:
							frame.X = 198;
							frame.Y = frameOffsetY + 126;
							break;
					}

					//mergeUp = true;
					//mergeLeft = true;
					//mergeRight = true;
				}
				else if (up == type && blendDown && blendLeft && blendRight)
				{
					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 198;
							frame.Y = frameOffsetY + 144;
							break;
						case 1:
							frame.X = 198;
							frame.Y = frameOffsetY + 162;
							break;
						default:
							frame.X = 198;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeDown = true;
					//mergeLeft = true;
					//mergeRight = true;
				}
				else if (blendUp && blendDown && left == type && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 216;
							frame.Y = frameOffsetY + 144;
							break;
						case 1:
							frame.X = 216;
							frame.Y = frameOffsetY + 162;
							break;
						default:
							frame.X = 216;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeUp = true;
					//mergeDown = true;
					//mergeRight = true;
				}
				else if (blendUp && blendDown && blendLeft && right == type)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 216;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 216;
							frame.Y = frameOffsetY + 108;
							break;
						default:
							frame.X = 216;
							frame.Y = frameOffsetY + 126;
							break;
					}

					//mergeUp = true;
					//mergeDown = true;
					//mergeLeft = true;
				}
				else if (blendUp && blendDown && blendLeft && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = frameOffsetY + 198;
							break;
						case 1:
							frame.X = 126;
							frame.Y = frameOffsetY + 198;
							break;
						default:
							frame.X = 144;
							frame.Y = frameOffsetY + 198;
							break;
					}

					//mergeUp = true;
					//mergeDown = true;
					//mergeLeft = true;
					//mergeRight = true;
				}
				else if (up == type && down == type && left == type && right == type)
				{
					if (typesToBlendWith.Contains(upLeft))
					{
						frameOffsetY = Array.IndexOf(typesToBlendWith, upLeft) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 18;
								frame.Y = frameOffsetY + 108;
								break;
							case 1:
								frame.X = 18;
								frame.Y = frameOffsetY + 144;
								break;
							default:
								frame.X = 18;
								frame.Y = frameOffsetY + 180;
								break;
						}
					}

					if (typesToBlendWith.Contains(upRight))
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, upRight) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 0;
								frame.Y = frameOffsetY + 108;
								break;
							case 1:
								frame.X = 0;
								frame.Y = frameOffsetY + 144;
								break;
							default:
								frame.X = 0;
								frame.Y = frameOffsetY + 180;
								break;
						}
					}

					if (typesToBlendWith.Contains(downLeft))
					{
						frameOffsetY = Array.IndexOf(typesToBlendWith, downLeft) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 18;
								frame.Y = frameOffsetY + 90;
								break;
							case 1:
								frame.X = 18;
								frame.Y = frameOffsetY + 126;
								break;
							default:
								frame.X = 18;
								frame.Y = frameOffsetY + 162;
								break;
						}
					}

					if (typesToBlendWith.Contains(downRight))
					{
						frameOffsetY = Array.IndexOf(typesToBlendWith, downRight) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 0;
								frame.Y = frameOffsetY + 90;
								break;
							case 1:
								frame.X = 0;
								frame.Y = frameOffsetY + 126;
								break;
							default:
								frame.X = 0;
								frame.Y = frameOffsetY + 162;
								break;
						}
					}
				}
			}
			else
			{

				if (up == -1 && blendDown && left == type && right == type)
				{
					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 234;
							frame.Y = frameOffsetY + 0;
							break;
						case 1:
							frame.X = 252;
							frame.Y = frameOffsetY + 0;
							break;
						default:
							frame.X = 270;
							frame.Y = frameOffsetY + 0;
							break;
					}

					//mergeDown = true;
				}
				else if (blendUp && down == -1 && left == type && right == type)
				{
					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 234;
							frame.Y = frameOffsetY + 18;
							break;
						case 1:
							frame.X = 252;
							frame.Y = frameOffsetY + 18;
							break;
						default:
							frame.X = 270;
							frame.Y = frameOffsetY + 18;
							break;
					}

					//mergeUp = true;
				}
				else if (up == type && down == type && left == -1 && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 234;
							frame.Y = frameOffsetY + 36;
							break;
						case 1:
							frame.X = 252;
							frame.Y = frameOffsetY + 36;
							break;
						default:
							frame.X = 270;
							frame.Y = frameOffsetY + 36;
							break;
					}

					//mergeRight = true;
				}
				else if (up == type && down == type && blendLeft && right == -1)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 234;
							frame.Y = frameOffsetY + 54;
							break;
						case 1:
							frame.X = 252;
							frame.Y = frameOffsetY + 54;
							break;
						default:
							frame.X = 270;
							frame.Y = frameOffsetY + 54;
							break;
					}

					//mergeLeft = true;
				}

				if (up != -1 && down != -1 && left == -1 && right == type)
				{
					if (blendUp && down == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;


						switch (variation)
						{
							case 0:
								frame.X = 72;
								frame.Y = frameOffsetY + 144;
								break;
							case 1:
								frame.X = 72;
								frame.Y = frameOffsetY + 162;
								break;
							default:
								frame.X = 72;
								frame.Y = frameOffsetY + 180;
								break;
						}

						//mergeUp = true;
					}
					else if (blendDown && up == type)
					{
						frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 72;
								frame.Y = frameOffsetY + 90;
								break;
							case 1:
								frame.X = 72;
								frame.Y = frameOffsetY + 108;
								break;
							default:
								frame.X = 72;
								frame.Y = frameOffsetY + 126;
								break;
						}

						//mergeDown = true;
					}
				}
				else if (up != -1 && down != -1 && left == type && right == -1)
				{
					if (blendUp && down == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 90;
								frame.Y = frameOffsetY + 144;
								break;
							case 1:
								frame.X = 90;
								frame.Y = frameOffsetY + 162;
								break;
							default:
								frame.X = 90;
								frame.Y = frameOffsetY + 180;
								break;
						}

						//mergeUp = true;
					}
					else if (blendDown && up == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 90;
								frame.Y = frameOffsetY + 90;
								break;
							case 1:
								frame.X = 90;
								frame.Y = frameOffsetY + 108;
								break;
							default:
								frame.X = 90;
								frame.Y = frameOffsetY + 126;
								break;
						}

						//mergeDown = true;
					}
				}
				else if (up == -1 && down == type && left != -1 && right != -1)
				{
					if (blendLeft && right == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 0;
								frame.Y = frameOffsetY + 198;
								break;
							case 1:
								frame.X = 18;
								frame.Y = frameOffsetY + 198;
								break;
							default:
								frame.X = 36;
								frame.Y = frameOffsetY + 198;
								break;
						}

						//mergeLeft = true;
					}
					else if (blendRight && left == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 54;
								frame.Y = frameOffsetY + 198;
								break;
							case 1:
								frame.X = 72;
								frame.Y = frameOffsetY + 198;
								break;
							default:
								frame.X = 90;
								frame.Y = frameOffsetY + 198;
								break;
						}

						//mergeRight = true;
					}
				}
				else if (up == type && down == -1 && left != -1 && right != -1)
				{
					if (blendLeft && right == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 0;
								frame.Y = frameOffsetY + 216;
								break;
							case 1:
								frame.X = 18;
								frame.Y = frameOffsetY + 216;
								break;
							default:
								frame.X = 36;
								frame.Y = frameOffsetY + 216;
								break;
						}

						//mergeLeft = true;
					}
					else if (blendRight && left == type)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;


						switch (variation)
						{
							case 0:
								frame.X = 54;
								frame.Y = frameOffsetY + 216;
								break;
							case 1:
								frame.X = 72;
								frame.Y = frameOffsetY + 216;
								break;
							default:
								frame.X = 90;
								frame.Y = frameOffsetY + 216;
								break;
						}

						//mergeRight = true;
					}
				}
				else if (up != -1 && down != -1 && left == -1 && right == -1)
				{
					if (blendUp && blendDown)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 108;
								frame.Y = frameOffsetY + 216;
								break;
							case 1:
								frame.X = 108;
								frame.Y = frameOffsetY + 234;
								break;
							default:
								frame.X = 108;
								frame.Y = frameOffsetY + 252;
								break;
						}

						//mergeUp = true;
						//mergeDown = true;
					}
					else if (blendUp)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 126;
								frame.Y = frameOffsetY + 144;
								break;
							case 1:
								frame.X = 126;
								frame.Y = frameOffsetY + 162;
								break;
							default:
								frame.X = 126;
								frame.Y = frameOffsetY + 180;
								break;
						}

						//mergeUp = true;
					}
					else if (blendDown)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 126;
								frame.Y = frameOffsetY + 90;
								break;
							case 1:
								frame.X = 126;
								frame.Y = frameOffsetY + 108;
								break;
							default:
								frame.X = 126;
								frame.Y = frameOffsetY + 126;
								break;
						}

						//mergeDown = true;
					}
				}
				else if (up == -1 && down == -1 && left != -1 && right != -1)
				{
					if (blendLeft && blendRight)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 162;
								frame.Y = frameOffsetY + 198;
								break;
							case 1:
								frame.X = 180;
								frame.Y = frameOffsetY + 198;
								break;
							default:
								frame.X = 198;
								frame.Y = frameOffsetY + 198;
								break;
						}

						//mergeLeft = true;
						//mergeRight = true;
					}
					else if (blendLeft)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 0;
								frame.Y = frameOffsetY + 252;
								break;
							case 1:
								frame.X = 18;
								frame.Y = frameOffsetY + 252;
								break;
							default:
								frame.X = 36;
								frame.Y = frameOffsetY + 252;
								break;
						}

						//mergeLeft = true;
					}
					else if (blendRight)
					{

						frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

						switch (variation)
						{
							case 0:
								frame.X = 54;
								frame.Y = frameOffsetY + 252;
								break;
							case 1:
								frame.X = 72;
								frame.Y = frameOffsetY + 252;
								break;
							default:
								frame.X = 90;
								frame.Y = frameOffsetY + 252;
								break;
						}

						//mergeRight = true;
					}
				}
				else if (blendUp && down == -1 && left == -1 && right == -1)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, up) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = frameOffsetY + 144;
							break;
						case 1:
							frame.X = 108;
							frame.Y = frameOffsetY + 162;
							break;
						default:
							frame.X = 108;
							frame.Y = frameOffsetY + 180;
							break;
					}

					//mergeUp = true;
				}
				else if (up == -1 && blendDown && left == -1 && right == -1)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, down) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 108;
							frame.Y = frameOffsetY + 90;
							break;
						case 1:
							frame.X = 108;
							frame.Y = frameOffsetY + 108;
							break;
						default:
							frame.X = 108;
							frame.Y = frameOffsetY + 126;
							break;
					}

					//mergeDown = true;
				}
				else if (up == -1 && down == -1 && blendLeft && right == -1)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, left) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 0;
							frame.Y = frameOffsetY + 234;
							break;
						case 1:
							frame.X = 18;
							frame.Y = frameOffsetY + 234;
							break;
						default:
							frame.X = 36;
							frame.Y = frameOffsetY + 234;
							break;
					}

					//mergeLeft = true;
				}
				else if (up == -1 && down == -1 && left == -1 && blendRight)
				{

					frameOffsetY = Array.IndexOf(typesToBlendWith, right) * 180;

					switch (variation)
					{
						case 0:
							frame.X = 54;
							frame.Y = frameOffsetY + 234;
							break;
						case 1:
							frame.X = 72;
							frame.Y = frameOffsetY + 234;
							break;
						default:
							frame.X = 90;
							frame.Y = frameOffsetY + frameOffsetY + 234;
							break;
					}

					//mergeRight = true;
				}
			}
			#endregion

			//BasicFraming(i, j);

			Main.tile[i, j].TileFrameX = (short)frame.X;
			Main.tile[i, j].TileFrameY = (short)frame.Y;
		}

		#endregion

		// TODO
		/*
		if (TileID.Sets.HasSlopeFrames[type])
		{
			int blockType = (int)tile.BlockType;

			if (blockType == 0)
			{
				bool upSlope = type == up && tileUp.TopSlope;
				bool leftSlope = type == left && tileLeft.LeftSlope;
				bool rightSlope = type == right && tileRight.RightSlope;
				bool downSlope = type == down && tileDown.BottomSlope;

				int slopeOffsetX = 0;
				int slopeOffsetY = 0;

				if (upSlope.ToInt() + leftSlope.ToInt() + rightSlope.ToInt() + downSlope.ToInt() > 2)
				{
					int slopeCount1 = (tileUp.Slope == SlopeType.SlopeDownLeft).ToInt() + (tileRight.Slope == SlopeType.SlopeDownLeft).ToInt() + (tileDown.Slope == SlopeType.SlopeUpRight).ToInt() + (tileLeft.Slope == SlopeType.SlopeUpRight).ToInt();
					int slopeCount2 = (tileUp.Slope == SlopeType.SlopeDownRight).ToInt() + (tileRight.Slope == SlopeType.SlopeUpLeft).ToInt() + (tileDown.Slope == SlopeType.SlopeUpLeft).ToInt() + (tileLeft.Slope == SlopeType.SlopeDownRight).ToInt();

					if (slopeCount1 == slopeCount2)
					{
						slopeOffsetX = 2;
						slopeOffsetY = 4;
					}
					else if (slopeCount1 > slopeCount2)
					{
						bool upLeftSolid = type == upLeft && tileUpLeft.Slope == SlopeType.Solid;
						bool downRightSolid = type == downRight && tileDownRight.Slope == SlopeType.Solid; ;
						if (upLeftSolid && downRightSolid)
						{
							slopeOffsetY = 4;
						}
						else if (downRightSolid)
						{
							slopeOffsetX = 6;
						}
						else
						{
							slopeOffsetX = 7;
							slopeOffsetY = 1;
						}
					}
					else
					{
						bool upRightSolid = type == upRight && tileUpRight.Slope == SlopeType.Solid;
						bool downLeftSolid = type == downLeft && tileDownLeft.Slope == SlopeType.Solid;
						if (upRightSolid && downLeftSolid)
						{
							slopeOffsetY = 4;
							slopeOffsetX = 1;
						}
						else if (downLeftSolid)
						{
							slopeOffsetX = 7;
						}
						else
						{
							slopeOffsetX = 6;
							slopeOffsetY = 1;
						}
					}

					frame.X = (18 + slopeOffsetX) * 18;
					frame.Y = frameOffsetY + frameOffsetY + slopeOffsetY * 18;
				}
				else
				{
					if (upSlope && leftSlope && type == down && type == right)
					{
						slopeOffsetY = 2;
					}
					else if (upSlope && rightSlope && type == down && type == left)
					{
						slopeOffsetX = 1;
						slopeOffsetY = 2;
					}
					else if (rightSlope && downSlope && type == up && type == left)
					{
						slopeOffsetX = 1;
						slopeOffsetY = 3;
					}
					else if (downSlope && leftSlope && type == up && type == right)
					{
						slopeOffsetY = 3;
					}

					if (slopeOffsetX != 0 || slopeOffsetY != 0)
					{
						frame.X = (18 + slopeOffsetX) * 18;
						frame.Y = frameOffsetY + slopeOffsetY * 18;
					}
				}
			}

			if (blockType >= 2 && (frame.X < 0 || frame.Y < 0))
			{
				int num40 = -1;
				int num41 = -1;
				int num42 = -1;
				int num43 = 0;
				int num44 = 0;
				switch (blockType)
				{
					case 2:
						num40 = left;
						num41 = down;
						num42 = downLeft;
						num43++;
						break;
					case 3:
						num40 = right;
						num41 = down;
						num42 = downRight;
						break;
					case 4:
						num40 = left;
						num41 = up;
						num42 = upLeft;
						num43++;
						num44++;
						break;
					case 5:
						num40 = right;
						num41 = up;
						num42 = upRight;
						num44++;
						break;
				}

				if (type != num40 || type != num41 || type != num42)
				{
					if (type == num40 && type == num41)
					{
						num43 += 2;
					}
					else if (type == num40)
					{
						num43 += 4;
					}
					else if (type == num41)
					{
						num43 += 4;
						num44 += 2;
					}
					else
					{
						num43 += 2;
						num44 += 2;
					}
				}

				frame.X = (18 + num43) * 18;
				frame.Y = frameOffsetY + num44 * 18;
			}
		}
		*/
	}
}
