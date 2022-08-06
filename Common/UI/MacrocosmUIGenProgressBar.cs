using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace Macrocosm.Common.UI
{
	public class MacrocosmUIGenProgressBar : UIGenProgressBar
	{

		private readonly Texture2D texUpper;
		private readonly Texture2D texLower;
		private readonly Texture2D fillLarge;
		private readonly Texture2D fillSmall;

		private float overallProgress;
		private float currentProgress;

		private Vector2 position;

		public void SetPosition(float x, float y) => position = new Vector2(x, y);

		public MacrocosmUIGenProgressBar(Texture2D texUpper, Texture2D texLower, Texture2D fillLarge, Texture2D fillSmall)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				this.texUpper = texUpper;
				this.texLower = texLower;
				this.fillLarge = fillLarge;
				this.fillSmall = fillSmall;
			}
			base.Recalculate();
		}

		public new void SetProgress(float overallProgress, float currentProgress)
		{
			this.overallProgress = overallProgress;
			this.currentProgress = currentProgress;
		}

		public new void DrawSelf(SpriteBatch spriteBatch)
		{
			if ((texUpper ?? texLower ?? fillLarge ?? fillSmall) != null)
			{

				DrawFilling(spriteBatch, new Rectangle((int)position.X + 20, (int)position.Y + 38, 568, 22), overallProgress, fillLarge, new Color(48, 48, 48));

				DrawFilling(spriteBatch, new Rectangle((int)position.X + 48, (int)position.Y + 60, 506, 12), currentProgress, fillSmall, new Color(48, 48, 48));

				Rectangle r = GetDimensions().ToRectangle();
				r.X -= 8;
				spriteBatch.Draw(texUpper, position + r.TopLeft(), Color.White);
				spriteBatch.Draw(texLower, position + r.TopLeft() + new Vector2(44f, 60f), Color.White);
			}
		}

		private static void DrawFilling(SpriteBatch spriteBatch, Rectangle rect, float progress, Texture2D texture, Color empty)
		{

			spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, empty);

			int steps = (int)((rect.Right - rect.Left) * progress);
			for (int i = 0; i < steps; i++)
			{
				spriteBatch.Draw(texture, new Rectangle(rect.Left + i, rect.Y, 1, rect.Height), Color.White);
			}
			spriteBatch.Draw(texture, new Rectangle(rect.X + (int)(progress * rect.Width), rect.Y, 2, rect.Height), null, Color.White * 0.5f);
		}
	}
}
