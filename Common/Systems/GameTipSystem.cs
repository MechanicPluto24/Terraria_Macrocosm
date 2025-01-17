﻿using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class GameTipSystem : ModSystem
    {
        public const string LoadingScreenTipsLocalizationPath = "Mods.Macrocosm.UI.LoadingScreenTips.";

        public override void Load()
        {
            IL_GameTipsDisplay.AddNewTip += IL_GameTipsDisplay_AddNewTip;
            On_GameTipsDisplay.GameTip.IsExpiring += GameTip_IsExpiring;
        }


        public override void Unload()
        {
            IL_GameTipsDisplay.AddNewTip -= IL_GameTipsDisplay_AddNewTip;
            On_GameTipsDisplay.GameTip.IsExpiring -= GameTip_IsExpiring;
        }


        /// <summary> IL Hook for manipulating game tips. </summary>
        private void IL_GameTipsDisplay_AddNewTip(ILContext il)
        {
            var c = new ILCursor(il);

            // matches the code that fetches a random language key
            // stored as a local variable (index 0) and then constructs a new GameTip 
            if (!c.TryGotoNext(
                i => i.MatchLdloc(out _),
                i => i.MatchStloc(out _),
                i => i.MatchLdarg(out _),
                i => i.MatchLdfld<GameTipsDisplay>("_currentTips"),
                i => i.MatchLdloc(out _), // <- loading the textKey
                i => i.MatchLdarg(out _)
            )) return;

            // move the cursor to the instruction that loads
            // the acquired language key from the evaluation stack
            c.Index += 5;

            // the key is captured from the stack by our hook,
            // either returning it back, or returning another language key, for custom game tips
            c.EmitDelegate(GetCustomTipText);
        }

        /// <summary> 
        /// Returns the localization key of the next game tip. Can return either one determined by our custom logic, 
        /// or return a random <paramref name="textKey"/>, determined by the game prior to this hook.
        /// <para>  Adding new Subworld LoadingScreen specific game tip has to be done in the localization files, under <c>LoadingScreensTips.SubworldClassName</c> </para>
        /// </summary>
        /// <param name="textKey"> The next game tip localization key </param>
        private string GetCustomTipText(string textKey)
        {
            // If a loading screen is currently drawing..
            if (LoadingScreen.CurrentlyActive)
            {
                // ..get the LoadingScreen specific GameTips, respective to the active subworld..
                LocalizedText[] messages = Language.FindAll(Lang.CreateDialogFilter(LoadingScreenTipsLocalizationPath + (SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current.Name : "Earth")));

                // .. and return a random message from that list
                if (messages.Length > 0)
                    return messages[Main.rand.Next(messages.Length)].Key;
            }

            // Otherwise, pass the original key and let the logic run unaffected
            return textKey;
        }

        /// <summary> Detour that ensures vanilla game tips will expire quickly when in a Macrocosm loading screen </summary>
        // TODO: this could use an IL edit instead of reflection
        private bool GameTip_IsExpiring(On_GameTipsDisplay.GameTip.orig_IsExpiring orig, object self, double currentTime)
        {
            if (!LoadingScreen.CurrentlyActive)
                return orig(self, currentTime);

            object textKey = self.GetType().GetFieldValue("_textKey", self);

            LocalizedText[] messages = Language.FindAll(Lang.CreateDialogFilter(LoadingScreenTipsLocalizationPath + (SubworldSystem.AnyActive<Macrocosm>() ? MacrocosmSubworld.Current.Name : "Earth")));
            if (textKey is LocalizedText text && !text.Key.Contains(LoadingScreenTipsLocalizationPath) && messages.Length > 0)
            {
                self.GetType().SetFieldValue("Duration", 1f, self);
                return true;
            }

            return orig(self, currentTime);
        }

        // This is called only when not in a loading screen.
        public override void UpdateUI(GameTime gameTime)
        {
            // Reset the flag that tells us that a current custom loading screen is drawing
            LoadingScreen.CurrentlyActive = false;
        }
    }
}