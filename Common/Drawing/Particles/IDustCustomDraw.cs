using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Common.Drawing.Particles
{
	public interface IDustCustomDraw
	{
		/// <summary> 
		/// Allows for custom dust drawing for this type,
		/// return false to stop the vanilla dust drawing code from running
		/// texture is the spritesheet of this dust type
		/// dustFrame is the current frame of the dust instance being drawn, use as sourceRectangle when drawing
		/// </summary>
		public bool DrawDust(SpriteBatch spriteBatch, Dust dust, Texture2D texture, Rectangle dustFrame);
	}
}
