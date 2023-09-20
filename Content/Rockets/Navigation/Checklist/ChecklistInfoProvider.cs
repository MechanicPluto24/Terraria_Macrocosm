using System.Collections.Generic;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
	public class ChecklistInfoProvider : IRocketDataConsumer
	{
		public Rocket Rocket { get; set; }
		public UIMapTarget MapTarget { get; set; }

		private UIListScrollablePanel panel;

		private ChecklistConditionCollection commonLaunchConditions = new();
		private ChecklistCondition selectedLaunchCondition;
		private ChecklistCondition hereLaunchCondition;

		public ChecklistInfoProvider()
		{
			selectedLaunchCondition = new ChecklistCondition("Selected", () => MapTarget is not null);
			hereLaunchCondition = new ChecklistCondition("NotHere", () => MapTarget is not null && !MapTarget.AlreadyHere);
			commonLaunchConditions.Add(new ChecklistCondition("Fuel", () => Rocket.Fuel >= Rocket.GetFuelCost(MapTarget.Name)));
			commonLaunchConditions.Add(new ChecklistCondition("Obstruction", () => Rocket.CheckFlightPathObstruction()));
		}

		public bool CheckLaunchConditions()
		{
			bool met = selectedLaunchCondition.IsMet() && hereLaunchCondition.IsMet() && commonLaunchConditions.MetAll();

			if (MapTarget is not null)
			{
				met &= MapTarget.CheckLaunchConditions();
				MapTarget.IsReachable = met;
			}

			return met;
		}

		public void Update()
		{
			panel.Deactivate();
			panel.ClearList();
			panel.AddRange(GetUpdatedChecklist());
			panel.Activate();
		}

		public UIListScrollablePanel ProvideUI()
		{
			panel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Checklist"), scale: 1.2f))
			{
				Width = new(0, 0.31f),
				BackgroundColor = new(53, 72, 135),
				BorderColor = new(89, 116, 213, 255)
			};

 
			panel.AddRange(GetUpdatedChecklist());
			return panel;
		}

		private List<UIElement> GetUpdatedChecklist()
		{
			List<UIElement> checklist = new();

			if (!selectedLaunchCondition.IsMet())
			{
				checklist.Add(selectedLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.QuestionMarkGold));
			}
			else if (!hereLaunchCondition.IsMet())
			{
				checklist.Add(hereLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.CrossmarkGray));
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