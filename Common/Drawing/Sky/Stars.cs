using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
    public class Stars : IEnumerable<MacrocosmStar>
    {
        private readonly List<MacrocosmStar> stars = new();

        public Vector2? CommonVelocity { get; set; } = null;

        public int Count => stars.Count;
        public bool None => Count == 0;
        public void Clear() => stars.Clear();

        public MacrocosmStar this[int index] => stars[index];
        public MacrocosmStar RandStar() => this[Main.rand.Next(Count - 1)];

        public Stars()
        {
        }

        public Stars(int minStars, int maxStars, Rectangle? area = null, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            SpawnStars(minStars, maxStars, area, baseScale, twinkleFactor);
        }

        private bool spawningDone = false;
        public void SpawnStars(int minStars, int maxStars, Rectangle? area = null, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            int count = Main.rand.Next(minStars, maxStars);
            Rectangle spawnArea;

            if (area.HasValue)
            {
                spawnArea = area.Value;
            }
            else
            {
                spawnArea = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            }

            for (int i = 0; i < count; i++)
            {
                int x = Main.rand.Next(spawnArea.Left, spawnArea.Right + 1);
                int y = Main.rand.Next(spawnArea.Top, spawnArea.Bottom + 1);

                Vector2 position = new Vector2(x, y);
                stars.Add(new MacrocosmStar(position, baseScale, twinkleFactor));
            }

            spawningDone = true;
        }

        public void SpawnStarsCelled(int minStars, int maxStars, bool falling = false, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            int count = Main.rand.Next(minStars, maxStars);

            int cellWidth = Main.screenWidth / (int)Math.Sqrt(count);
            int cellHeight = Main.screenHeight / (int)Math.Sqrt(count);

            for (int i = 0; i < count; i++)
            {
                int cellX = i % (Main.screenWidth / cellWidth);
                int cellY = i / (Main.screenWidth / cellWidth);

                int posX = cellX * cellWidth + Main.rand.Next(cellWidth);
                int posY = cellY * cellHeight + Main.rand.Next(cellHeight);

                Vector2 starPosition = new(posX, posY);
                stars.Add(new MacrocosmStar(starPosition, baseScale, twinkleFactor));
            }
        }

        public void DrawAll(SpriteBatch spriteBatch, float brightness = 1f)
        {
            DrawStationary(spriteBatch, brightness);
            DrawFalling(spriteBatch, brightness);
        }

        public void DrawStationary(SpriteBatch spriteBatch, float brightness = 1f)
        {
            if (None || !spawningDone)
                return;

            foreach (MacrocosmStar star in stars)
            {
                if (star.Falling)
                    continue;

                star.Brightness = brightness;

                if (CommonVelocity.HasValue)
                    star.Velocity = CommonVelocity.Value;

                star.Update();
                star.Draw(spriteBatch);
            }
        }

        public void DrawFalling(SpriteBatch spriteBatch, float brightness = 1f)
        {
            if (None || !spawningDone)
                return;

            foreach (MacrocosmStar star in stars)
            {
                if (!star.Falling)
                    continue;

                star.Brightness = brightness;

                if (CommonVelocity.HasValue)
                    star.Velocity = CommonVelocity.Value;

                star.Update();
                star.Draw(spriteBatch);
            }
        }

        public IEnumerator<MacrocosmStar> GetEnumerator()
        {
            return ((IEnumerable<MacrocosmStar>)stars).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)stars).GetEnumerator();
        }
    }
}