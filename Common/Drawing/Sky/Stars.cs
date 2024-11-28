using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
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

        public Vector2 CommonOffset { get; set; } = Vector2.Zero;
        public Vector2? CommonVelocity { get; set; } = null;

        public int Count => stars.Count;
        public bool None => Count == 0;
        public void Clear() => stars.Clear();

        public MacrocosmStar this[int index] => stars[index];
        public MacrocosmStar RandStar() => Count > 0 ? this[Main.rand.Next(Count - 1)] : new MacrocosmStar(default);

        public Stars()
        {
        }

        public Stars(int count, Rectangle? area = null, bool randomColor = false, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            SpawnStars(count, area, randomColor, baseScale, twinkleFactor);
        }

        public Stars(RawTexture colorMap, int count, Rectangle? area = null, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            SpawnStars(colorMap, count, area, baseScale, twinkleFactor);
        }

        private bool spawningDone = false;
        public void SpawnStars(int count, Rectangle? area = null, bool randomColor = false, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
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
                Vector2 position = new(x, y);

                Color[] starColors =
                [
                    new Color(255, 244, 232), // White 
                    new Color(255, 210, 161), // Yellow 
                    new Color(255, 180, 107), // Orange 
                    new Color(255, 128, 64),  // Red-orange 
                    new Color(255, 102, 102), // Red 
                    new Color(170, 191, 255), // Light Blue 
                    new Color(100, 149, 237), // Blue
                ];

                Color? color = null;
                if(randomColor && Main.rand.NextBool(5))
                     color = starColors[Main.rand.Next(starColors.Length)];

                stars.Add(new MacrocosmStar(position, baseScale, twinkleFactor, color));
            }

            spawningDone = true;
        }

        public void SpawnStars(RawTexture colorMap, int count, Rectangle? area = null, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            Rectangle spawnArea = area ?? new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            int starsSpawned = 0;
            int maxAttempts = count * 10; 
            int attempts = 0;

            while (starsSpawned < count && attempts < maxAttempts)
            {
                attempts++;

                int x = Main.rand.Next(spawnArea.Left, spawnArea.Right + 1);
                int y = Main.rand.Next(spawnArea.Top, spawnArea.Bottom + 1);

                float mapX = Terraria.Utils.Remap(x, spawnArea.Left, spawnArea.Right, 0, colorMap.Width - 1);
                float mapY = Terraria.Utils.Remap(y, spawnArea.Top, spawnArea.Bottom, 0, colorMap.Height - 1);

                int clampedMapX = Math.Clamp((int)mapX, 0, colorMap.Width - 1);
                int clampedMapY = Math.Clamp((int)mapY, 0, colorMap.Height - 1);

                Color pixelColor = colorMap[clampedMapX, clampedMapY];
                float chance = pixelColor.GetBrightness();

                if (Main.rand.NextFloat(1f) <= chance)
                {
                    Vector2 position = new(x, y); 
                    stars.Add(new MacrocosmStar(position, baseScale, twinkleFactor, pixelColor));
                    starsSpawned++;
                }
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
                star.Draw(spriteBatch, CommonOffset);
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
                star.Draw(spriteBatch, CommonOffset);
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