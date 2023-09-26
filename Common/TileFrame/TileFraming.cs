
using Macrocosm.Common.DataStructures;
using Terraria;

namespace Macrocosm.Common.TileFrame
{
	public class TileFraming
	{
		// Framing legend:
		// ---------------
		//  The tile   | T	
		//  Solid      | #	
		//  Empty      | _
		//  Don't care |  
		// ---------------

		public static bool PlatingStyle(ushort type, int i, int j)
		{
			Tile tile = Main.tile[i, j];
			var info = new TileNeighbourInfo(i, j).TypedSolid(type);
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

			return shouldRunBaseFraming;
		}
	}
}
