using Macrocosm.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria.UI;
using System.Linq;
using Terraria;

namespace Macrocosm.Common.Hooks
{
	internal class UIElementDetours : ILoadable
	{
		private Dictionary<string, IFocusable> focusedElementsByContext = new Dictionary<string, IFocusable>();

		public void Load(Mod mod)
		{
			On_UIElement.Draw += On_UIElement_Draw;
			On_UIElement.Update += On_UIElement_Update;
			On_UIElement.OnInitialize += On_UIElement_OnInitialize;
		}

		public void Unload()
		{
			On_UIElement.Draw -= On_UIElement_Draw;
			On_UIElement.Update -= On_UIElement_Update;
			On_UIElement.OnInitialize -= On_UIElement_OnInitialize;
		}

		private void On_UIElement_OnInitialize(On_UIElement.orig_OnInitialize orig, UIElement self)
		{
			orig(self);
			TrackUIElement(self);
		}

		private void On_UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
		{
			orig(self, spriteBatch);
		}

		private void On_UIElement_Update(On_UIElement.orig_Update orig, UIElement self, Microsoft.Xna.Framework.GameTime gameTime)
		{

			try
			{
				if (self is IFocusable focusable && focusable.FocusContext is not null)
				{
					IFocusable current;

					if (focusable.HasFocus)
					{
						if (focusedElementsByContext.TryGetValue(focusable.FocusContext, out current))
						{
							if (current != focusable)
							{
								current.HasFocus = false;
								current.OnFocusLost();

								focusable.OnFocusGain();
								focusedElementsByContext[focusable.FocusContext] = focusable;
							}
						}
						else
						{
							focusable.OnFocusGain();
							focusedElementsByContext[focusable.FocusContext] = focusable;
						}
					}
					else if (focusedElementsByContext.TryGetValue(focusable.FocusContext, out current))
					{
						if (current == focusable)
						{
							focusedElementsByContext.Remove(focusable.FocusContext);
							focusable.OnFocusLost?.Invoke();
						}
					}
				}
			} 
			catch (Exception e)
			{
				Main.NewText(e.Message);
			}
			
			orig(self, gameTime);
		}

		private void TrackUIElement(UIElement uIElement)
		{
			 
 
		}
	}
}
