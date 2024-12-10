using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    public class UICargoFuelPreview : UIPanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        private UIText title;

        private UILiquidTank rocketFuelTank; 
        private UITextPanel<string> textPanel;


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
                Height = new(0, 0.75f),
                Top = new(0, 0.1f),
                HAlign = 0.5f,
                WaveAmplitude = 1f,
                WaveFrequency = 2f
            };
            Append(rocketFuelTank);

            textPanel = new("", textScale: 0.8f)
            {
                Width = new(0, 0.8f),
                Height = new(0, 0.14f),
                Top = new(0, 0.875f),
                HAlign = 0.5f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            textPanel.SetPadding(4f);
            textPanel.PaddingTop += 4f;
            textPanel.PaddingLeft -= 4f;
            Append(textPanel);
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

            string target = Main.LocalPlayer.GetModPlayer<RocketPlayer>().TargetWorld;
            float fuelCost = string.IsNullOrEmpty(target) ? 0f : RocketFuelLookup.GetFuelCost(MacrocosmSubworld.CurrentID, target);
            float clampedCost = MathHelper.Clamp(fuelCost, 0, Rocket.FuelCapacity);

            float availableFuelLevel = Rocket.Fuel / Rocket.FuelCapacity;
            float consumedFuelLevel = clampedCost / Rocket.FuelCapacity;

            textPanel.SetText(fuelCost > 0f ? $"-{fuelCost}" : "?");
            textPanel.TextColor = fuelCost > Rocket.Fuel ? Color.Red : Color.White;

            rocketFuelTank.LiquidLevel = MathHelper.Lerp(rocketFuelTank.LiquidLevel, availableFuelLevel - consumedFuelLevel, 0.1f);
            rocketFuelTank.PreviewLiquidLevel = MathHelper.Lerp(rocketFuelTank.PreviewLiquidLevel, availableFuelLevel, 0.1f);
        }
    }
}
