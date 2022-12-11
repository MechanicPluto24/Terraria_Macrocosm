using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
	public class UINavigationMap : UIElement
	{
		public Texture2D Background;

		public UINavigationMap Next = null;
		public UINavigationMap Prev = null;
		public UINavigationMap DefaultNext = null;

		private Dictionary<UIMapTarget, UINavigationMap> nextTargetChildMap = new();

		private bool showAnimationActive = false;
		private Texture2D animationPrevTexture;

		float alpha = 1f;
		float alphaSpeed = 0.02f;

		public UINavigationMap(Texture2D tex, UINavigationMap next = null, UINavigationMap prev = null, UINavigationMap defaultNext = null)
		{
			Background = tex;

			Next = next;
			Prev = prev;
			DefaultNext = defaultNext;
		}

		public void AddTarget(UIMapTarget target, UINavigationMap childMap = null)
		{
			if (childMap is not null)
				nextTargetChildMap.Add(target, childMap);

			Append(target);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (nextTargetChildMap.Count > 0)
			{
				UIMapTarget target = GetSelectedTarget();

				if (target != null && nextTargetChildMap.TryGetValue(target, out UINavigationMap navMap))
					Next = navMap;
				else
					Next = null;
			}

			if (showAnimationActive)
			{
				alpha += alphaSpeed;

				if (alpha >= 1f)
				{
					showAnimationActive = false;
					alpha = 1f;
				}
			}
		}

		public UIMapTarget GetSelectedTarget()
		{
			if (showAnimationActive)
				return null;

			foreach (UIElement element in Children)
			{
				if (element is UIMapTarget target && target.Selected)
					return target;
			}

			return null;
		}

		public void ClearAllTargets()
		{
			foreach (UIElement element in Children)
			{
				if (element is UIMapTarget target)
					target.Selected = false;
			}
		}

		public override void OnInitialize()
		{
			Width.Set(Background.Width, 0);
			Height.Set(Background.Height, 0);
			HAlign = 0.5f;
			VAlign = 0.5f;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			bool specialDraw = animationPrevTexture is not null && showAnimationActive;

			spriteBatch.Draw(TextureAssets.BlackTile.Value, GetDimensions().ToRectangle(), Color.Black);

			SpriteBatchState state = new();
			if (specialDraw)
			{
				state = spriteBatch.SaveState();
				spriteBatch.End();
				spriteBatch.Begin(BlendState.NonPremultiplied, state);

				spriteBatch.Draw(animationPrevTexture, dimensions.Position(), null, Color.White.NewAlpha(1 - alpha), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}

			spriteBatch.Draw(Background, dimensions.Position(), null, Color.White.NewAlpha(alpha), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			if (specialDraw)
				spriteBatch.Restore(state);
		}

		public void ShowAnimation(Texture2D previousTexture)
		{
			showAnimationActive = true;
			animationPrevTexture = previousTexture;
			alpha = 0f;
			ClearAllTargets();
		}
	}
}