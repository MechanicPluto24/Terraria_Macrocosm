using System.Collections.Generic;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Content.Rockets.Navigation.NavigationPanel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class UIFlightChecklist : UIListScrollablePanel, IRocketDataConsumer
	{
		public Rocket Rocket { get; set; }
		public UIMapTarget MapTarget { get; set; }

		private ChecklistConditionCollection commonLaunchConditions = new();
		private ChecklistCondition selectedLaunchCondition;

		public UIFlightChecklist() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Checklist"), scale: 1.2f))
		{
			selectedLaunchCondition = new ChecklistCondition("Selected", () => MapTarget is not null);
			commonLaunchConditions.Add(new ChecklistCondition("Fuel", () => Rocket.Fuel >= Rocket.GetFuelCost(MapTarget.Name)));
			commonLaunchConditions.Add(new ChecklistCondition("Obstruction", () => Rocket.CheckFlightPathObstruction()));
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			BackgroundColor = new(53, 72, 135);
			BorderColor = new(89, 116, 213, 255);
		}

		public bool CheckLaunchConditions()
		{
			bool met = selectedLaunchCondition.IsMet() && commonLaunchConditions.MetAll();

			if (MapTarget is not null)
			{
				met &= MapTarget.CheckLaunchConditions();
				MapTarget.IsReachable = met;
			}

			return met;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Deactivate();
			ClearList();
			AddRange(GetUpdatedChecklist());
			Activate();
		}

		private List<UIElement> GetUpdatedChecklist()
		{
			List<UIElement> checklist = new();

			if (!selectedLaunchCondition.IsMet())
			{
				checklist.Add(selectedLaunchCondition.ProvideUIInfoElement(ChecklistInfoElement.ExtraIconType.QuestionMarkGold));
			}
			else
			{
				if (MapTarget.LaunchConditions is not null)
					checklist.AddRange(MapTarget.LaunchConditions.ProvideUIElementList());

				checklist.AddRange(commonLaunchConditions.ProvideUIElementList());
			}

			return checklist;
		}
	}
}