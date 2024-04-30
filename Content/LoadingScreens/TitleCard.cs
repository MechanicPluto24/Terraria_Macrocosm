using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.LoadingScreens
{
    public class TitleCard : ModSystem
    {
        enum TitleState
        {
            Inactive,
            FadingIn,
            Active,
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
            targetWorld = MacrocosmSubworld.SanitizeID(targetWorld, out _);

            title = targetWorld switch
            {
                "Moon" => new(Language.GetText("Mods.Macrocosm.Subworlds.Moon.DisplayName"), Color.White, 1.2f, largeText: true),
                "Earth" => new(Language.GetText("Mods.Macrocosm.Subworlds.Earth.DisplayName"), new Color(94, 150, 255), 1.2f, largeText: true),
                _ => null,
            };
        }

        public static void Start()
        {
            timer = 0;
            titleFadeValue = 0f;
            currentState = TitleState.FadingIn;
        }

        public static void Update()
        {
            switch (currentState)
            {
                case TitleState.Inactive:
                    break;

                case TitleState.FadingIn:
                    titleFadeValue += titleFadeRate;
                    if (titleFadeValue >= 1f)
                    {
                        titleFadeValue = 1f;
                        currentState = TitleState.Active;
                        timer = 0;
                    }
                    break;

                case TitleState.Active:
                    timer++;
                    if (timer >= 90)
                    {
                        currentState = TitleState.FadingOut;
                    }
                    break;

                case TitleState.FadingOut:
                    titleFadeValue -= titleFadeRate;
                    if (titleFadeValue <= 0f)
                         currentState = TitleState.Inactive;
                    break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if(currentState is not TitleState.Inactive)
                title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.2f), title.Color * titleFadeValue);
        }
    }
}
