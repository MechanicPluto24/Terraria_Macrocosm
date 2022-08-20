using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Common.Drawing
{
	public class StarsDrawing
	{
		private readonly List<MacrocosmStar> stars = new();

		public int Count => stars.Count;
		public bool None => Count == 0;
		public void Clear() => stars.Clear();

		public MacrocosmStar this[int index] => stars[index];
		public MacrocosmStar StarAt(int index) => this[index];
		public MacrocosmStar RandStar() => this[Main.rand.Next(Count - 1)];

		public void SpawnStars(int minStars, int maxStars, float baseScale = 1f, float twinkleFactor = 0.4f)
		{
			int count = Main.rand.Next(minStars, maxStars);

			for (int i = 0; i < count; i++)
			{
				stars.Add(new MacrocosmStar(baseScale, twinkleFactor));
			}
		}

		public void DrawSelf(SpriteBatch spriteBatch, float brightness = 1f)
		{
			if (None)
				return;

			foreach (MacrocosmStar star in stars)
			{
				star.Brightness = brightness;
				star.Update();
				star.DrawSelf(spriteBatch);
			}
		}
	}
}