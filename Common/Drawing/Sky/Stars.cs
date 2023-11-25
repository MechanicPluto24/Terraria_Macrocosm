using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Common.Drawing.Sky
{
    public class Stars : IEnumerable<MacrocosmStar>
    {
        private readonly List<MacrocosmStar> stars = new();

        public Vector2 MovementVector { get; set; }

        public int Count => stars.Count;
        public bool None => Count == 0;
        public void Clear() => stars.Clear();

        public MacrocosmStar this[int index] => stars[index];
        public MacrocosmStar RandStar() => this[Main.rand.Next(Count - 1)];

        public Stars()
        {
        }

        public Stars(int minStars, int maxStars, bool celledSpawn = false, MacrocosmStar.WrapMode wrapMode = MacrocosmStar.WrapMode.None, bool falling = false, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            SpawnStars(minStars, maxStars, celledSpawn, wrapMode, falling, baseScale, twinkleFactor);
        }

        private bool spawningDone = false;
        public void SpawnStars(int minStars, int maxStars, bool celledSpawn = false, MacrocosmStar.WrapMode wrapMode = MacrocosmStar.WrapMode.None, bool falling = false, float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            int count = Main.rand.Next(minStars, maxStars);

            if (celledSpawn)
            {
                int cellWidth = Main.screenWidth / (int)Math.Sqrt(count);
                int cellHeight = Main.screenHeight / (int)Math.Sqrt(count);

                for (int i = 0; i < count; i++)
                {
                    int cellX = i % (Main.screenWidth / cellWidth);
                    int cellY = i / (Main.screenWidth / cellWidth);

                    int posX = cellX * cellWidth + Main.rand.Next(cellWidth);
                    int posY = cellY * cellHeight + Main.rand.Next(cellHeight);

                    Vector2 starPosition = new(posX, posY);

                    if (falling)
                        stars.Add(new FallingStar(starPosition, baseScale, twinkleFactor, wrapMode));
                    else
                        stars.Add(new MacrocosmStar(starPosition, baseScale, twinkleFactor, wrapMode));
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (falling)
                        stars.Add(new FallingStar(baseScale, twinkleFactor, wrapMode));
                    else
                        stars.Add(new MacrocosmStar(baseScale, twinkleFactor, wrapMode));
                }
            }

            spawningDone = true;
        }

        public void Draw(SpriteBatch spriteBatch, float brightness = 1f)
        {
            if (None || !spawningDone)
                return;

            foreach (MacrocosmStar star in stars)
            {
                star.Brightness = brightness;
                star.UpdatePosition(MovementVector);
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