using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenShapes
{
	public class HollowRectangle : GenShape
	{
		private Rectangle area;

		public HollowRectangle(Rectangle area)
		{
			this.area = area;
		}

		public HollowRectangle(int width, int height)
		{
			area = new Rectangle(0, 0, width, height);
		}

		public void SetArea(Rectangle area)
		{
			this.area = area;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			int minX = origin.X + area.Left;
			int maxX = origin.X + area.Right;
			int minY = origin.Y + area.Top;
			int maxY = origin.Y + area.Bottom;

			for (int i = minX; i < maxX; i++)
			{
				for (int j = minY; j < maxY; j++)
				{
					bool isOnBoundary = i == minX || i == maxX - 1 || j == minY || j == maxY - 1;

					if (isOnBoundary)
					{
						if (!UnitApply(action, origin, i, j) && _quitOnFail)
							return false;
					}
				}
			}

			return true;
		}
	}
}