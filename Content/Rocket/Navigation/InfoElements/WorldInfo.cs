using Terraria.Localization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
{
    public class WorldInfo
    {
        private string worldNameKey;
        private List<BasicInfoElement> elements;

        public void Add(BasicInfoElement element) { elements.Add(element); }

        public WorldInfo(string worldNameKey)
        {
            this.worldNameKey = worldNameKey;
        }

        public WorldInfo(string worldNameKey, List<BasicInfoElement> elements)
        {
            this.worldNameKey = worldNameKey;
            this.elements = elements;
        }

        const string localizationPath = "Mods.Macrocosm.Subworlds.";
        public UIInfoPanel ProvideUI()
        {
            UIInfoPanel panel = new(Language.GetTextValue(localizationPath + worldNameKey + ".DisplayName"));

            string flavorText = Utility.GetLanguageValueOrEmpty(localizationPath + worldNameKey + ".FlavorText");

            if (flavorText != string.Empty && flavorText != "default")
            {
                panel.Add(new UIFlavorTextPanel(flavorText));
                AppendSeparator(panel);
            }

            if (elements is not null)
            {
                bool foundHazards = false;
                foreach (BasicInfoElement element in elements)
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

        private void AppendSeparator(UIInfoPanel panel)
        {
            panel.Add(new UIHorizontalSeparator()
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
                Color = new Color(89, 116, 213, 255) * 0.9f
            });
        }
    }
}
