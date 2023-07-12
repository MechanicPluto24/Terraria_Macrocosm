using System;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
	public class ChecklistCondition
	{
		public string LangKey { get; private set; }

		private readonly Func<bool> predicate = () => false;

		public bool HideIfMet = true;

		private readonly ChecklistInfoElement checklistInfoElement;

		public ChecklistCondition(string langKey, Func<bool> canLaunch, bool hideIfMet = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			HideIfMet = hideIfMet;

			checklistInfoElement = new(langKey);
		}

		public virtual bool Met() => predicate();

		public virtual UIElement ProvideUI()
		{
			checklistInfoElement.State = Met();
			UIElement uIElement = checklistInfoElement.ProvideUI();
			uIElement.Height = new StyleDimension(46f, 0f);
			return uIElement;
		}

		public virtual UIElement ProvideUI(ChecklistInfoElement.IconType notMetIcon = ChecklistInfoElement.IconType.Crossmark, ChecklistInfoElement.IconType metIcon = ChecklistInfoElement.IconType.Checkmark)
		{
			checklistInfoElement.NotMetIcon = notMetIcon;
			checklistInfoElement.MetIcon = metIcon;
			return ProvideUI();
		}
	}
}
