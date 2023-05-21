using Terraria.UI;
using Terraria.Localization;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConds
{
    public class LaunchCondition
    {
		public delegate bool FuncCanLaunch();
		private readonly FuncCanLaunch CanLaunch = () => false;

		public LaunchCondition(string langKey, FuncCanLaunch canLaunch)
        {
            LangKey = langKey;
            CanLaunch = canLaunch;
            checklistInfoElement = new(langKey);
        }

        public string LangKey { get; private set; }

        public virtual bool Check() => CanLaunch();

        public virtual UIElement ProvideUI() => checklistInfoElement.ProvideUI();

        private ChecklistInfoElement checklistInfoElement;
    }
}
