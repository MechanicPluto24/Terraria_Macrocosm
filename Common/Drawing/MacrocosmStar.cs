using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing
{
    public class MacrocosmStar : Star
    {
        public Texture2D texture;

        public MacrocosmStar(float baseScale)
        {
            FastRandom fastRandom = FastRandom.CreateWithRandomSeed();
            texture = TextureAssets.Star[fastRandom.Next(0, 4)].Value;

            // TODO: X position check in the star group like done in the Star.SpawnStars

            position.X = fastRandom.Next(1921);
            position.Y = fastRandom.Next(1201);
            rotation = (float)fastRandom.Next(628) * 0.01f;
            scale = (float)fastRandom.Next(70, 130) * 0.006f * baseScale;
            // type = fastRandom.Next(0, 4); -- not used 
            twinkle = (float)fastRandom.Next(60, 101) * 0.01f;
            twinkleSpeed = (float)fastRandom.Next(30, 110) * 0.0001f;

            if (fastRandom.Next(2) == 0)
                twinkleSpeed *= -1f;

            rotationSpeed = (float)fastRandom.Next(5, 50) * 0.0001f;
            if (fastRandom.Next(2) == 0)
                rotationSpeed *= -1f;

            if (fastRandom.Next(40) == 0)
            {
                scale *= 2f;
                twinkleSpeed /= 2f;
                rotationSpeed /= 2f;
            }
        }

        public MacrocosmStar(float baseScale, Texture2D tex) : this(baseScale)
        {
            texture = tex;
        }

        public MacrocosmStar(float baseScale, int vanillaTextureType) : this(baseScale)
        {
            texture = TextureAssets.Star[vanillaTextureType].Value;
        }
    }
}
