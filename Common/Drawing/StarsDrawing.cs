using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;
using Macrocosm.Content;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Macrocosm.Common.Drawing
{
    public class StarsDrawing
    {

        List<MacrocosmStar> stars = new();

        public bool None => stars.Count == 0;
        public void Clear() => stars.Clear();

        public void Draw(SpriteBatch spriteBatch)
        {
            if (None)
                return;

            foreach(MacrocosmStar star in stars)
            {
                spriteBatch.Draw(star.texture, star.position, null, Color.White, star.rotation, star.texture.Size() / 2, star.scale, default, 0f);
                star.Update();
            }
        }

        public void SpawnStars(int minStars, int maxStars, float baseScale)
        {
            int count = Main.rand.Next(minStars, maxStars);

            for (int i = 0; i < count; i++)
            {
                stars.Add(new MacrocosmStar(baseScale));
            }
        }
    }
}