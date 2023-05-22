using Terraria.UI;
using Terraria.Localization;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConds
{
    public class LaunchCondition
    {
		public delegate bool FuncCanLaunch();
		private readonly FuncCanLaunch CanLaunch = () => false;

        private readonly ChecklistInfoElement checklistInfoElement;

		public LaunchCondition(string langKey, FuncCanLaunch canLaunch)
        {
            LangKey = langKey;
            CanLaunch = canLaunch;
            checklistInfoElement = new(langKey);
        }

        public string LangKey { get; private set; }

        public virtual bool Check() => CanLaunch();

		public virtual UIElement ProvideUI()
		{
			checklistInfoElement.State = Check();
			return checklistInfoElement.ProvideUI();
		}

        public virtual UIElement ProvideUI(ChecklistInfoElement.IconType falseIcon = ChecklistInfoElement.IconType.Crossmark, ChecklistInfoElement.IconType trueIcon = ChecklistInfoElement.IconType.Checkmark)
        {
            checklistInfoElement.FalseIcon = falseIcon;
            checklistInfoElement.TrueIcon = trueIcon;
            return ProvideUI();
		}
	}
}
