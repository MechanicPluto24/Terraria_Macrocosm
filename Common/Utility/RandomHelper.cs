using System;
using System.Collections.Generic;

namespace Macrocosm.Common.Utility
{
	[Obsolete("PickRandom() can potentially get stuck. Also... Why is there a static field...")]
	public static class RandomHelper
	{
		// Oh yeah >:)
		public static T PickRandom<T>(T[] input)
		{
			int rand = new Random().Next(0, input.Length);

			return input[rand];
		}
		private static List<int> chosenTs = new List<int>();
		public static List<T> PickRandom<T>(this T[] input, int amount)
		{
			List<T> values = new List<T>();
			for (int i = 0; i < amount; i++)
			{
			ReRoll:
				int rand = new Random().Next(0, input.Length);

				if (!chosenTs.Contains(rand))
				{
					chosenTs.Add(rand);
					values.Add(input[rand]);
				}
				else
					goto ReRoll;
			}
			chosenTs.Clear();
			return values;
		}
	}
}