

using Macrocosm.Common.Utility;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIWorldInfoPanel : UIPanel
	{
		public string Name = "";

		private UIText UIDisplayName;
		private UIList UIInfoElements;
		
		public override void OnInitialize()
		{
			Width.Set(245, 0);
			Height.Set(400, 0);
			Left.Set(10, 0f);
			Top.Set(249, 0f);
			SetPadding(0f);
			BorderColor = new Color(89, 116, 213, 255);
			BackgroundColor = new Color(73, 94, 171);

			UIList uIList = new UIList
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
			};

			uIList.SetPadding(2f);
			uIList.PaddingBottom = 4f;
			uIList.PaddingTop = 50f;
			uIList.PaddingLeft = 6f;
			Append(uIList);
			UIInfoElements = uIList;

			uIList.ListPadding = 4f;
			uIList.ManualSortMethod = (_) => { };

			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(150f, 1000f);
			uIScrollbar.Height.Set(0f, 0.95f);
			uIScrollbar.HAlign = 0.99f;
			uIScrollbar.VAlign = 0.5f;
			UIInfoElements.SetScrollbar(uIScrollbar);

			Append(uIScrollbar);
			UIInfoElements.Width.Set(-20f, 1f);

  			UIDisplayName = new(Name, 1.5f, false)
			{
				HAlign = 0.43f,
				Top = new StyleDimension(15, 0f),
				TextColor = Color.White
			};
			Append(UIDisplayName);

		}

		public void FillWithInfo(SubworldData data)
		{
			UIInfoElements.Clear();

			string iconPath = "Macrocosm/Content/Rocket/UI/Icons/";

			UIHorizontalSeparator uIHorizontalSeparator = new UIHorizontalSeparator
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
				Color = new Color(89, 116, 213, 255) * 0.9f
			};

			string gravData = data.SpecialGravity != "" ? data.SpecialGravity : data.Gravity > 0 ? data.Gravity.ToString() + " G" : "";
			if (gravData != "")
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "Gravity").Value, gravData, "Gravity"));

			string radiusData = data.SpecialRadius != "" ? data.SpecialRadius : data.Radius > 0 ? data.Radius.ToString() + " km": "";
			if (radiusData != "")
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "Radius").Value, radiusData, "Radius"));

			string dayLenghtData = data.SpecialDayPeriod != "" ? data.SpecialDayPeriod : data.DayPeriod > 0 ? data.DayPeriod.ToString() + " days" : "";
			if (dayLenghtData != "")
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "DayPeriod").Value, dayLenghtData, "Day period"));

			string threatData = data.ThreatLevel > 0 ? data.ThreatLevel.ToString() : "";
			if (threatData != "")
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "ThreatLevel").Value, threatData, "Threat Level"));

			if(data.Hazards.Count > 0)
			{
				UIInfoElements.Add(uIHorizontalSeparator);

				foreach (var hazard in data.Hazards)
				{
					UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + hazard.Key).Value, hazard.Value, "Hazard"));
				}
			}
			
 			UIInfoElements.Recalculate();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UIDisplayName.SetText(Name);
		}
	}
}
