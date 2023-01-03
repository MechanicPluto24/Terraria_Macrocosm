using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Common.Drawing.Dusts
{
	public interface IDustCustomDraw
	{
		/// <summary>
		/// Allows for custom dust drawing for this type, return false to stop the vanilla dust drawing code from running.  
		/// Substract <c>Main.screenPosition</c> from <c>dust.position</c> before drawing.
		/// </summary>
		/// <param name="spriteBatch"> The spritebatch </param>
		/// <param name="dust"> The dust instance </param>
		/// <param name="texture"> The spritesheet of this dust type </param>
		/// <param name="dustFrame"> The frame of this dust instance. Use as <c>sourceRectangle</c> </param>
		/// <returns> Whether to allow for vanilla drawing for this dust type </returns>
		public bool DrawDust(SpriteBatch spriteBatch, Dust dust, Texture2D texture, Rectangle dustFrame);
	}
}
