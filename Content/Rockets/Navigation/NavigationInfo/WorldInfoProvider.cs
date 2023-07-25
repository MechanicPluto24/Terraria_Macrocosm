using Terraria.Localization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Macrocosm.Common.UI;
using Humanizer;
using Terraria;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    public class WorldInfoProvider
    {
        private readonly string worldNameKey;
        private readonly List<InfoElement> elements;

        public void Add(InfoElement element) { elements.Add(element); }

        public WorldInfoProvider(string worldNameKey)
        {
            this.worldNameKey = worldNameKey;
        }

        public WorldInfoProvider(string worldNameKey, List<InfoElement> elements)
        {
            this.worldNameKey = worldNameKey;
            this.elements = elements;
        }

        private const string localizationPath = "Mods.Macrocosm.Subworlds.";

        public UIListScrollablePanel ProvideUI()
        {
            UIListScrollablePanel panel = new(new LocalizedColorScaleText(Language.GetText(localizationPath + worldNameKey + ".DisplayName"), scale: 1.2f))
            {
                Width = new StyleDimension(0, 0.31f),
                Height = new StyleDimension(0, 0.62f),
                Left = new StyleDimension(0, 0.01f),
                Top = new StyleDimension(0, 0.365f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			panel.SetPadding(0f);
			
            LocalizedText flavorText = Utility.GetLocalizedTextOrEmpty(localizationPath + worldNameKey + ".FlavorText");

            if (flavorText != LocalizedText.Empty && flavorText.Value != "default")
            {
                panel.Add(new UIDynamicTextPanel(new LocalizedColorScaleText(flavorText, Color.White, scale: 0.85f)));
                AppendSeparator(panel);
            }

            if (elements is not null)
            {
                bool foundHazards = false;
                foreach (InfoElement element in elements)
                {
                    if (!foundHazards && element is HazardInfoElement)
                    {
                        AppendSeparator(panel);
                        foundHazards = true;
                    }

                    panel.Add(element.ProvideUI());
                }
            }

            return panel;
        }

        private void AppendSeparator(UIListScrollablePanel panel)
        {
            panel.Add(new UIHorizontalSeparator()
            {
                Width = StyleDimension.FromPercent(0.98f),
                Color = new Color(89, 116, 213, 255) * 0.9f
            });
        }
    }
}
