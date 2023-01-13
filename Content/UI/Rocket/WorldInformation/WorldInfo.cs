using Terraria.Localization;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket.WorldInformation
{
	public class WorldInfo
	{
		private string worldNameKey;
		private List<WorldInfoElement> elements { get; set; }

		public void Add(WorldInfoElement element) { elements.Add(element); }
		
		public WorldInfo(string worldNameKey)
		{
			this.worldNameKey = worldNameKey;
		}

		public WorldInfo(string worldNameKey, List<WorldInfoElement> elements)
		{
			this.worldNameKey = worldNameKey;
			this.elements = elements;
		}

		string localizationPath = "Mods.Macrocosm.Worlds.";
		public UIWorldInfoPanel ProvideUI()
		{
			UIWorldInfoPanel panel = new(Language.GetTextValue(localizationPath + worldNameKey + ".DisplayName"));

			string flavorText = Utility.GetLanguageValueOrEmpty(localizationPath + worldNameKey + ".FlavorText");	

			if (flavorText != string.Empty)
			{
				panel.Add(new UIWorldInfoTextPanel(flavorText));
				AppendSeparator(panel);
			}

			if(elements is not null)
			{
				bool foundHazards = false;
				foreach (WorldInfoElement element in elements)
				{
					if (!foundHazards && element.InfoType == InfoType.Hazard)
					{
						AppendSeparator(panel);
						foundHazards = true;
					}

					panel.Add(element.ProvideUI());
				}
			}

			return panel;
		}

		private void AppendSeparator(UIWorldInfoPanel panel)
		{
			panel.Add(new UIHorizontalSeparator()
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
				Color = new Color(89, 116, 213, 255) * 0.9f
			});
		}
	}
}
