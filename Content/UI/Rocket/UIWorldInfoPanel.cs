using Macrocosm.Common.Subworlds.WorldInformation;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
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

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UIDisplayName.SetText(Name);
		}

		public void ClearInfo() => UIInfoElements.Clear();

		public void FillWithInfo(WorldInfo info)
		{
			UIInfoElements.Clear();

			string iconPath = "Macrocosm/Content/UI/Rocket/Icons/";
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

			if (info.FlavorText != "")
			{
				UIInfoElements.Add(new UIWorldInfoTextPanel(info.FlavorText));
				UIInfoElements.Add(new UIHorizontalSeparator()
				{
					Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
					Color = new Color(89, 116, 213, 255) * 0.9f
				});
			}

			if (info.Gravity != "" || info.Gravity > 0f)
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "Gravity", mode).Value,
					info.Gravity, "Gravity", info.Gravity.GetUnitText()));

			if (info.Radius != "" || info.Radius > 0f)
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "Radius", mode).Value,
					info.Radius, "Radius", info.Radius.GetUnitText()));

			if (info.DayPeriod != "" || info.DayPeriod > 0f)
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "DayPeriod", mode).Value,
					info.DayPeriod, "Day period", info.DayPeriod.GetUnitText(info.DayPeriod == 1)));

			if (info.ThreatLevel > 0)
				UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + "ThreatLevel", mode).Value,
					info.GetThreatLevelCombined(), "Threat Level", valueColor: info.GetThreatColor()));

			if (info.Hazards.Count > 0)
			{

				UIInfoElements.Add(new UIHorizontalSeparator()
				{
					Width = StyleDimension.FromPixelsAndPercent(0f, 0.98f),
					Color = new Color(89, 116, 213, 255) * 0.9f
				});

				foreach (string hazardKey in info.GetHazardKeys())
				{
					UIInfoElements.Add(new UIWorldInfoElement(ModContent.Request<Texture2D>(iconPath + hazardKey, mode).Value, info.Hazards[hazardKey], "Hazard"));
				}
			}

			UIInfoElements.Recalculate();
		}
	}
}
