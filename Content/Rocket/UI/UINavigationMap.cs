using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UINavigationMap : UIElement
	{
		public Texture2D Background;
		
		public UINavigationMap Next = null;
		public UINavigationMap Prev = null;

		private Dictionary<UIMapTarget, UINavigationMap> nextTargetChildMap = new();

		public UINavigationMap(Texture2D tex, UINavigationMap next = null, UINavigationMap prev = null)
		{
			Background = tex;
			
			Next = next;
			Prev = prev;
		}

		public void AddTarget(UIMapTarget target, UINavigationMap childMap = null)
		{
			if(childMap is not null)
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
		}

		public UIMapTarget GetSelectedTarget()
		{
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
			spriteBatch.Draw(Background, dimensions.ToRectangle(), Color.White);
		}
	}
}
