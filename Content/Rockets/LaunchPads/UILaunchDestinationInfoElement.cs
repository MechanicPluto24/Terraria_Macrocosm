using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public class UILaunchDestinationInfoElement : UIInfoElement, IFocusable
    {
        public LaunchPad LaunchPad { get; init; }

        public bool HasFocus { get; set; }
        public string FocusContext { get; set; }
        public Action OnFocusGain { get; set; }
        public Action OnFocusLost { get; set; }

        public bool IsSpawnPointDefault => LaunchPad is null;

        public bool IsCurrent { get; set; }
        public bool IsReachable { get; set; }

        public bool CanInteract { get; set; } = true;

        public UILaunchDestinationInfoElement() : base(
            Language.GetText("Mods.Macrocosm.UI.LaunchPad.UnknownLocation"),
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "QuestionMark", AssetRequestMode.ImmediateLoad),
            null,
            null
        )
        {
            Width = new(0f, 1f);
            Height = new(40f, 0f);
            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;
        }

        public UILaunchDestinationInfoElement(LaunchPad launchPad) : base(
            $"{launchPad.DisplayName} ({launchPad.CompassCoordinates})",
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "LaunchPad", AssetRequestMode.ImmediateLoad),
            null,
            null
        )
        {
            LaunchPad = launchPad;
            Width = new(0f, 1f);
            Height = new(40f, 0f);
            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            if (uIDisplayText is not null)
                uIDisplayText.TextColor = UITheme.Current.CommonTextColor;

            if (IsMouseHovering)
            {
                BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
                BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
            }

            if (HasFocus)
            {
                BackgroundColor = UITheme.Current.ButtonStyle.BackgroundColor;
                BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
            }

            if (!IsReachable || IsCurrent)
            {
                BackgroundColor = Color.Lerp(UITheme.Current.PanelStyle.BackgroundColor, Color.DarkGray, 0.1f);
            }
        }
    }
}
