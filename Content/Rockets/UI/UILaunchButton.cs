using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UILaunchButton : UIPanel
    {
        public delegate void OnClick_ZoomIn(bool useDefault);
        public OnClick_ZoomIn ZoomIn = (useDefault) => { };

        public delegate void OnClick_Launch();
        public OnClick_Launch Launch = () => { };

        public bool CanClick;

        private UIText buttonText;

        public enum StateType
        {
            NoTarget,
            CantReach,
            AlreadyHere,
            ZoomIn,
            Launch,
            LaunchInactive
        }
        public StateType ButtonState;

        public override void OnInitialize()
        {
            Width.Set(0, 0.34f);
            Height.Set(0, 0.1f);
            HAlign = 0.5f;
            Top.Set(0, 0.885f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            Recalculate();

            buttonText = new(LocalizedText.Empty, 0.9f, true)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.5f,
                TextColor = Color.White
            };

            Append(buttonText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            OnLeftClick -= UILaunchButton_OnClick_Launch;
            OnLeftClick -= UILaunchButton_OnClick_ZoomIn;
            CanClick = true;

            LocalizedText text = LocalizedText.Empty;
            Color textColor = Color.White;

            // TODO: add dynamic scaling
            float textScale = 0.9f;

            text = Language.GetText("Mods.Macrocosm.UI.Rocket.LaunchButton." + ButtonState.ToString());

            switch (ButtonState)
            {
                case StateType.NoTarget:
                    textColor = Color.Gold;
                    CanClick = false;
                    break;

                case StateType.CantReach:
                    textColor = Color.Red;
                    textScale = 0.75f;
                    CanClick = false;
                    break;

                case StateType.AlreadyHere:
                    textColor = Color.Gray * 1.3f;
                    textScale = 0.58f;
                    CanClick = false;
                    break;

                case StateType.ZoomIn:
                    textColor = Color.White;
                    textScale = 1.05f;
                    OnLeftClick += UILaunchButton_OnClick_ZoomIn;
                    break;

                case StateType.Launch:
                    textColor = new Color(0, 255, 0);
                    textScale = 1.1f;
                    OnLeftClick += UILaunchButton_OnClick_Launch;
                    break;

                case StateType.LaunchInactive:
                    textColor = new Color(0, 200, 0);
                    textScale = 1.1f;
                    CanClick = false;
                    break;

                default:
                    CanClick = false;
                    break;
            }


            buttonText.TextColor = textColor;
            buttonText.SetText(text, textScale, true);

            if (!CanClick)
            {
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor * 0.75f;
                BorderColor = UITheme.Current.PanelStyle.BorderColor;
            }
            else if (IsMouseHovering)
            {
                BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
                BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
            }
            else
            {
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
                BorderColor = UITheme.Current.PanelStyle.BorderColor;
            }

            BackgroundColor.A = 255;
        }

        private void UILaunchButton_OnClick_ZoomIn(UIMouseEvent evt, UIElement listeningElement)
        {
            ZoomIn(false);
        }

        private void UILaunchButton_OnClick_Launch(UIMouseEvent evt, UIElement listeningElement)
        {
            Launch();
        }
    }
}
