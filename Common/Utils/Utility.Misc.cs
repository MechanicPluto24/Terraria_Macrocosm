using System;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Common.Utils
{
    internal static partial class Utility
    {
		public static T GetRandom<T>(this IList<T> list)
			=> list[Main.rand.Next(list.Count)];

		public static bool IsAprilFools() 
			=> DateTime.Now.Month == 4 && DateTime.Now.Day == 1;
    }
}