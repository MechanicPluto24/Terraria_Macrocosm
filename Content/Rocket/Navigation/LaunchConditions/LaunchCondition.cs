using Terraria.UI;
using Terraria.Localization;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConditions
{
    public class LaunchCondition
    {
		public delegate bool FuncCanLaunch();
		private readonly FuncCanLaunch CanLaunch = () => false;

		public LaunchCondition(string langKey, FuncCanLaunch canLaunch)
        {
            CanLaunch = canLaunch;
            checklistInfoElement = new(langKey);
        }

        public virtual void CheckCondition() => CanLaunch();

        public virtual UIElement ProvideUI() => checklistInfoElement.ProvideUI();

        private ChecklistInfoElement checklistInfoElement;
    }
}
