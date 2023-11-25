using Macrocosm.Common.Config;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.LoadingScreens
{
    public class LoadingTitleSequence : ModSystem
    {
        enum TitleState
        {
            Inactive,
            FadingToBlack,
            Black,
            FadingTitleIn,
            TitleShown,
            FadingOut
        }

        private static LocalizedColorScaleText title;
        private static TitleState currentState = TitleState.Inactive;
        private static float timer = 0;
        private static float titleFadeValue = 0f;
        private const float titleFadeRate = 0.01f;

        public override void Load()
        {
        }

        public override void Unload()
        {
            title = null;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (currentState != TitleState.Inactive)
            {
                if ((Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient) && currentState != TitleState.Inactive)
                    Update();

                Draw(spriteBatch);
            }
        }

        public static void SetTargetWorld(string targetWorld)
        {
            title = targetWorld switch
            {
                "Moon" => new(Language.GetText("Mods.Macrocosm.Subworlds.Moon.DisplayName"), Color.White, 1.2f, largeText: true),
                "Earth" => new(Language.GetText("Mods.Macrocosm.Subworlds.Earth.DisplayName"), new Color(94, 150, 255), 1.2f, largeText: true),
                _ => null,
            };
        }

        public static void Start(bool noTitle)
        {
            timer = 0;
            titleFadeValue = 0f;
            //FadeEffect.StartFadeIn(0.01f);
            currentState = TitleState.Black;

            if (noTitle)
                title = null;
        }

        public static void Update()
        {
            switch (currentState)
            {
                case TitleState.Inactive:
                    break;

                case TitleState.FadingToBlack:
                    break;

                case TitleState.Black:
                    timer++;
                    if (timer >= 30) // 0.5 seconds
                    {
                        if (title is null)
                        {
                            FadeEffect.ResetFade();
                            FadeEffect.StartFadeIn(0.01f);
                            currentState = TitleState.FadingOut;
                        }
                        else
                        {
                            currentState = TitleState.FadingTitleIn;
                        }

                    }
                    break;

                case TitleState.FadingTitleIn:
                    titleFadeValue += titleFadeRate;
                    if (titleFadeValue >= 1f)
                    {
                        titleFadeValue = 1f;
                        currentState = TitleState.TitleShown;
                        timer = 0;
                    }
                    break;

                case TitleState.TitleShown:
                    timer++;
                    if (timer >= 90) // 1.5 seconds
                    {
                        currentState = TitleState.FadingOut;
                        FadeEffect.ResetFade();
                        FadeEffect.StartFadeIn(0.01f);
                    }
                    break;

                case TitleState.FadingOut:
                    titleFadeValue -= titleFadeRate;
                    if (titleFadeValue <= 0f)
                    {
                        titleFadeValue = 0f;
                    }
                    if (!FadeEffect.IsFading && titleFadeValue == 0f)
                    {
                        currentState = TitleState.Inactive;
                    }
                    break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            switch (currentState)
            {
                case TitleState.FadingToBlack:
                    //MacrocosmSubworld.LoadingScreen?.Draw(gameTime, spriteBatch);
                    //FadeEffect.Draw();
                    break;

                case TitleState.Black:
                case TitleState.FadingTitleIn:
                case TitleState.TitleShown:
                    FadeEffect.DrawBlack(1f);
                    title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.2f), title.Color * titleFadeValue);
                    break;

                case TitleState.FadingOut:
                    FadeEffect.Draw();
                    title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.2f), title.Color * titleFadeValue);
                    break;
            }
        }
    }
}
