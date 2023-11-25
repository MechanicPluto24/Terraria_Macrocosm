using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class UIFlightChecklist : UIListScrollablePanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }
        public UINavigationTarget MapTarget { get; set; }

        public bool AllMet { get; set; }

        private ChecklistConditionCollection commonLaunchConditions = new();
        private ChecklistCondition selectedLaunchCondition;

        public UIFlightChecklist() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Checklist"), scale: 1.2f))
        {
            selectedLaunchCondition = new ChecklistCondition("Selected", () => MapTarget is not null);

            commonLaunchConditions.Add(new ChecklistCondition("Fuel", () => Rocket.Fuel >= Rocket.GetFuelCost(MapTarget.Name)));

            // NOTE: This must be kept as an explicit lambda expression!
#pragma warning disable IDE0200
            commonLaunchConditions.Add(new ChecklistCondition("Obstruction", () => Rocket.CheckFlightPathObstruction(), checkPeriod: 10));
#pragma warning restore IDE0200

            commonLaunchConditions.Add(new ChecklistCondition("Boss", () => !Utility.BossActive && !Utility.MoonLordIncoming, hideIfMet: true));
            commonLaunchConditions.Add(new ChecklistCondition("Invasion", () => !Utility.InvastionActive && !Utility.PillarsActive, hideIfMet: true));
            commonLaunchConditions.Add(new ChecklistCondition("BloodMoon", () => !Utility.BloodMoonActive, hideIfMet: true));
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool anyConditionChanged = false;
            bool allConditionsMet = true;

            bool ProcessCondition(ChecklistCondition condition)
            {
                bool wasMet = condition.Check();
                anyConditionChanged |= condition.HasChanged;
                allConditionsMet &= wasMet;
                return wasMet;
            }

            if (ProcessCondition(selectedLaunchCondition))
            {
                foreach (var condition in commonLaunchConditions)
                    ProcessCondition(condition);

                if (MapTarget != null)
                {
                    allConditionsMet &= MapTarget.CheckLaunchConditions();
                    if (MapTarget.LaunchConditions != null)
                        foreach (var condition in MapTarget.LaunchConditions)
                            ProcessCondition(condition);

                    MapTarget.IsReachable = allConditionsMet;
                }
            }

            if (anyConditionChanged)
            {
                Deactivate();
                ClearList();
                AddRange(GetUpdatedChecklist());
                Activate();
            }

            AllMet = allConditionsMet;
        }

        private List<UIElement> GetUpdatedChecklist()
        {
            List<UIElement> uIChecklist = new();
            ChecklistConditionCollection checklistConditions = new();

            if (!selectedLaunchCondition.IsMet)
            {
                checklistConditions.Add(selectedLaunchCondition);
            }
            else
            {
                if (MapTarget.LaunchConditions is not null)
                    checklistConditions.AddRange(MapTarget.LaunchConditions);

                checklistConditions.AddRange(commonLaunchConditions);
            }

            var sortedConditions = checklistConditions
                .Where(condition => !(condition.IsMet && condition.HideIfMet))
                .OrderBy(condition => condition.IsMet).ToList();

            foreach (var condition in sortedConditions)
                uIChecklist.Add(condition.ProvideUIInfoElement());

            return uIChecklist;
        }
    }
}