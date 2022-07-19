using Terraria;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Drawing.Stars
{
    public class StarsDrawing
    {

        private readonly List<MacrocosmStar> stars = new();

        public bool None => stars.Count == 0;
        public void Clear() => stars.Clear();

        public void SpawnStars(int minStars, int maxStars, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            int count = Main.rand.Next(minStars, maxStars);

            for (int i = 0; i < count; i++)
            {
                stars.Add(new MacrocosmStar(baseScale, twinkleFactor));
            }
        }

        public void Draw(float brightness = 1f)
        {
            if (None)
                return;

            foreach (MacrocosmStar star in stars)
            {
                star.brightness = brightness;
                star.Update();
                star.Draw();
            }
        }

   
    }
}