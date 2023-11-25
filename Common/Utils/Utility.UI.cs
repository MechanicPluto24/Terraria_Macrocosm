using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Normalized UI scale, ranges from 0.0f to 1.0f </summary>
		public static float NormalizedUIScale => (Main.UIScale - 0.5f) / (2.0f - 0.5f);

        /// <summary>  Removes all chidren of this element that match a predicate </summary>
        /// <param name="element"> The element being processed </param>
        /// <param name="match"> The predicate to filter the elements </param>
        public static void RemoveAllChildrenWhere(this UIElement element, Func<UIElement, bool> match)
        {
            List<UIElement> childrenToRemove = new();

            foreach (UIElement child in element.Children)
                if (match(child))
                    childrenToRemove.Add(child);

            foreach (UIElement childToRemove in childrenToRemove)
                element.RemoveChild(childToRemove);
        }

        /// <summary> Gets a list of all the children of this element that match a predicate </summary>
        /// <param name="element"> The element being processed </param>
        /// <param name="match"> The predicate to filter the elements </param>
        public static List<UIElement> GetChildrenWhere(this UIElement element, Func<UIElement, bool> match)
        {
            List<UIElement> matchedChildren = new();

            foreach (UIElement child in element.Children)
                if (match(child))
                    matchedChildren.Add(child);

            return matchedChildren;
        }

        /// <summary> Recursively gets a list of all the children (and children of children, and so on) in the tree, that match a predicate </summary>
        /// <param name="element"> The element being processed </param>
        /// <param name="match"> The predicate to filter the elements </param>
        // Did not test this lol -- Feldy
        public static List<UIElement> GetChildrenRecursivelyWhere(this UIElement element, Func<UIElement, bool> match)
        {
            List<UIElement> matchedChildren = new();

            element.ExecuteRecursively(child =>
            {
                if (match(child))
                    matchedChildren.Add(child);
            });

            return matchedChildren;
        }

        /// <summary> 
        /// Schedules the replace of a child UIElement from this parent with another, on the next update tick. <br> 
        /// If replacing the same element with an updated version, make sure you also update the reference of 
        /// the old element so it points to the new element before calling this method. </br>
        /// </summary>
        /// <param name="parent"> The parent </param>
        /// <param name="toRemove"> The child to be replaced </param>
        /// <param name="newElement"> The element to replace the child with </param>
        /// <param name="updateReference"> Whether to also update the old element reference with the new one </param>
        // This is preferred instead of immediate replacement since, if the method is called while updating, where 
        // the children are also iterated upon, the children collection would be modified, causing an exception
        public static void ReplaceChildWith(this UIElement parent, UIElement toRemove, UIElement newElement)
        {
            UIElement.ElementEvent replaceHandler = null;

            // This handler is called before the actual updating takes place
            replaceHandler = new UIElement.ElementEvent((element) =>
            {
                //int childrenCount = element.Children.Count();

                if (element.HasChild(toRemove))
                    element.RemoveChild(toRemove);

                element.Append(newElement);
                newElement.Activate();

                //if (childrenCount != element.Children.Count())
                //{
                //	Chat("Failed to replace child properly. Make sure you also update the reference of newElement if replacing the same element with an updated version.", Color.Yellow);
                //	Macrocosm.Instance.Logger.Warn("Failed to replace child properly. Make sure you also update the reference of newElement if replacing the same element with an updated version.");
                //}

                // Detach this handler once complete, we don't want it to run every tick from now on
                element.OnUpdate -= replaceHandler;
            });

            // If method called during or after the Update method, this schedules the replacement in the next tick
            parent.OnUpdate += replaceHandler;
        }

        /// <summary> Whether the <paramref name="key"/> has just been pressed for this <paramref name="keyState"/> </summary>
        public static bool KeyPressed(this KeyboardState keyState, Keys key) => keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);

        /// <summary> Whether the <paramref name="key"/> has just been release for this <paramref name="keyState"/> </summary>
        public static bool KeyReleased(this KeyboardState keyState, Keys key) => !keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyDown(key);

        /// <summary> Whether the <paramref name="key"/> has just been pressed in the <see cref="Main.keyState"/> context </summary>
        public static bool KeyPressed(Keys key) => Main.keyState.KeyPressed(key);

        /// <summary> Whether the <paramref name="key"/> has just been released in the <see cref="Main.keyState"/> context </summary>
        public static bool KeyReleased(Keys key) => Main.keyState.KeyReleased(key);

        /// <summary> Common player conditions that usually trigger the closing of UIs on screen </summary>
        public static bool UICloseConditions(this Player player) =>
            player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !Main.playerInventory;

        /// <summary> Closes common vanilla UIs, used when bringing up another UI </summary>
        public static void UICloseOthers()
        {
            Player player = Main.LocalPlayer;

            if (player.sign > -1)
            {
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = string.Empty;
            }

            if (Main.editChest)
            {
                Main.editChest = false;
                Main.npcChatText = string.Empty;
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }

            if (player.talkNPC > -1)
            {
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = string.Empty;
            }
        }
    }
}
