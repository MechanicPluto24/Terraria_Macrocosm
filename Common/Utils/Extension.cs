using Macrocosm.Content.CameraModifiers;
using Macrocosm.Content.Items.Global;
using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Players;
using Macrocosm.Content.Projectiles.Global;
using Macrocosm.Content.Rockets;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Macrocosm.Common.Utils
{
	public static class Extension
    {
		public static int[] ToIntArray(this Range range, int length = int.MaxValue)
		{
			int start = range.Start.GetOffset(length);
			int end = range.End.GetOffset(length);

			int[] indices = new int[end + 1 - start];
			for (int i = start; i <= end; i++)
 				indices[i - start] = i;
 
			return indices;
		}

		public static bool Contains(this Range range, int index, int length = int.MaxValue)
		{
			int start = range.Start.GetOffset(length);
			int end = range.End.GetOffset(length);

			return index >= start && index < end;
		}

		public static bool Contains(this Range range, Index index, int length = int.MaxValue)
		{
			int start = range.Start.GetOffset(length);
			int end = range.End.GetOffset(length);

			int actualIndex = index.GetOffset(length);

			return actualIndex >= start && actualIndex < end;
		}

		public static void AddVariations(this FlexibleTileWand rubbleMaker, int itemType, int tileIdToPlace, Range styles)
		{
			rubbleMaker.AddVariations(itemType, tileIdToPlace, styles.ToIntArray());
		}
	}
}
