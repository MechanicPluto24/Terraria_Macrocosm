using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    public class UICargoFuelPreview : UIPanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        private UIText title;

        private UILiquidTank rocketFuelTank;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(0, 0.124f);
            Height.Set(0, 0.336f);
            HAlign = 0f;
            Top.Set(0, 0.01f);
            Left.Set(0, 0.865f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            title = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Cargo"), 0.8f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.005f,
                TextColor = Color.White
            };
            Append(title);

            rocketFuelTank = new(LiquidType.RocketFuel)
            {
                Width = new(0, 0.8f),
                Height = new(0, 0.9f),
                Top = new(0, 0.1f),
                HAlign = 0.5f,
                WaveAmplitude = 1f,
                WaveFrequency = 2f
            };
            Append(rocketFuelTank);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
                BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
            }
            else
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor;
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            }

            rocketFuelTank.LiquidLevel = Rocket.Fuel / Rocket.FuelCapacity;
        }
    }
}
