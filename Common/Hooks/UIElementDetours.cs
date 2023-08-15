using Macrocosm.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria.UI;
using System.Linq;
using Terraria;
using Macrocosm.Common.Utils;

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
 		}

		private void On_UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
		{
			orig(self, spriteBatch);
		}

		private void On_UIElement_Update(On_UIElement.orig_Update orig, UIElement self, Microsoft.Xna.Framework.GameTime gameTime)
		{
			orig(self, gameTime);
			HandleFocus(self);
		}

		private void HandleFocus(UIElement element)
		{
			if (element is IFocusable focusable && focusable.FocusContext is not null)
			{
				IFocusable current;

				// If current element should have focus
				if (focusable.HasFocus)
				{
					// In the case where there is an element in this context which already has focus...
					if (focusedElementsByContext.TryGetValue(focusable.FocusContext, out current))
					{
						// ... and it's not this element:
						if (current != focusable)
						{
							// Lose focus for that other element
							current.HasFocus = false;
							current.OnFocusLost();

							// Gain focus for this one
							focusedElementsByContext[focusable.FocusContext] = focusable;
							focusable.OnFocusGain();
						}
					}
					// In the case where there are no focused elements in this context:
					else
					{
						// Gain focus for this one
						focusedElementsByContext[focusable.FocusContext] = focusable;
						focusable.OnFocusGain();
					}
				}
				// In the case where this element does not have focus...
				else if (focusedElementsByContext.TryGetValue(focusable.FocusContext, out current))
				{
					// ... but just had focus in this context:
					if (current == focusable)
					{
						// Lose focus for this one
						focusedElementsByContext.Remove(focusable.FocusContext);
						focusable.OnFocusLost();
					}
				}
			}
		}
	}
}
