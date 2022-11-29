using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Earth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIMapTarget : UIElement
	{
		public SubworldData TargetWorldData;
		
		public delegate bool FuncSelectable();
		private readonly FuncSelectable isSelectable;

		public bool Selected;
		public bool Hover;

		public UIMapTarget(Vector2 position, float width, float height, FuncSelectable selectable = default, SubworldData targetWorldData = default)
		{
			TargetWorldData = targetWorldData;

			isSelectable = selectable;

			Width.Set(width, 0);
			Height.Set(height, 0);
			Top.Set(position.Y, 0);
			Left.Set(position.X, 0);
		}

		public override void OnInitialize()
		{
			OnClick += (_,_) =>
			{
				if (isSelectable())
				{
					foreach (UIElement element in Parent.Children)
					{
						if (element is UIMapTarget target && !ReferenceEquals(target, this))
							target.Selected = false;
					}
					Selected = true;
				}
			};

			OnRightClick += (_, _) => { Selected = false; };

			OnDoubleClick += (_, _) => { (Parent.Parent as UINavigationPanel).ZoomIn(); };
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle rect = GetDimensions().ToRectangle();
			rect.Inflate(15, 15);

			SpriteBatchState state = spriteBatch.SaveState();
			spriteBatch.EndIfBeginCalled();
			spriteBatch.Begin(BlendState.NonPremultiplied, state);

			Texture2D outline = ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UISelectionOutline").Value;

			if (Selected)
 				spriteBatch.Draw(outline, rect, Color.Green);
			else if(Hover)
				spriteBatch.Draw(ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/UI/UISelectionOutline").Value, rect, Color.Gold);

			spriteBatch.Restore(state);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			Hover = isSelectable();
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			Hover = isSelectable();
		}
	}
}
