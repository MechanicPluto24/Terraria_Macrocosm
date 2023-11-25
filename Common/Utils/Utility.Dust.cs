using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Gets the dust spritesheet of this vanilla dust type as a rectangle </summary>
        public static Rectangle VanillaDustFrame(int dustType)
        {
            int frameX = dustType * 10 % 1000;
            int frameY = dustType * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            return new Rectangle(frameX, frameY, 8, 8);
        }

        /// <summary> 
        /// Get the spritesheet of this dust instance.
        /// Use with the <c>dust.frame</c> of this instace as  <c>sourceRectangle</c> when drawing 
        /// </summary>
        public static Texture2D GetTexture(this Dust dust)
             => GetDustTexture(dust.type);

        /// <summary> 
        /// Get the spritesheet of this particular dust type.  
        /// Use with <c>dust.frame</c> as <c>sourceRectangle</c> when drawing.
        /// </summary>
        public static Texture2D GetDustTexture(int dustType)
             => ModContent.GetModDust(dustType).Texture2D.Value;

    }
}
