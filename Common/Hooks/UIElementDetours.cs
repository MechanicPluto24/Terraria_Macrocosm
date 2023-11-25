using Macrocosm.Common.UI;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Hooks
{
    public class UIElementDetours : ILoadable
    {
        private static Dictionary<string, IFocusable> focusedElementsByContext;

        public void Load(Mod mod)
        {
            On_UIElement.Update += On_UIElement_Update;
            focusedElementsByContext = new();
        }

        public void Unload()
        {
            On_UIElement.Update -= On_UIElement_Update;
            focusedElementsByContext = null;
        }

        private void On_UIElement_Update(On_UIElement.orig_Update orig, UIElement self, Microsoft.Xna.Framework.GameTime gameTime)
        {
            orig(self, gameTime);
            HandleFocus(self);
        }

        private void HandleFocus(UIElement element)
        {
            // TODO: call OnFocusGain/Lost even if the element does not have a focus context 
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
                            current.OnFocusLost?.Invoke();

                            // Gain focus for this one
                            focusedElementsByContext[focusable.FocusContext] = focusable;
                            focusable.OnFocusGain?.Invoke();
                        }
                    }
                    // In the case where there are no focused elements in this context:
                    else
                    {
                        // Gain focus for this one
                        focusedElementsByContext[focusable.FocusContext] = focusable;
                        focusable.OnFocusGain?.Invoke();
                    }
                }
                // In the case where this element should not have focus...
                else if (focusedElementsByContext.TryGetValue(focusable.FocusContext, out current))
                {
                    // ... but still has focus in this context:
                    if (current == focusable)
                    {
                        // Lose focus for this one
                        focusedElementsByContext.Remove(focusable.FocusContext);
                        focusable.OnFocusLost?.Invoke();
                    }
                }
            }
        }
    }
}
