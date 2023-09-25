using Macrocosm.Common.UI;
using System;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
	public class ChecklistCondition
	{
		public string LangKey { get; private set; }
		public bool HideIfMet = true;

		private readonly Func<bool> predicate = () => false;
		private bool cachedMet;
		private int checkCounter;
		private int checkPeriod;

		private readonly ChecklistInfoElement checklistInfoElement;

		public ChecklistCondition(string langKey, Func<bool> canLaunch, int checkPeriod = 1,  bool hideIfMet = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			this.checkPeriod = checkPeriod;
			HideIfMet = hideIfMet;

			checklistInfoElement = new(langKey);
		}

		public virtual bool IsMet()
		{
			checkCounter++;

			if (checkCounter >= checkPeriod)
			{
				checkCounter = 0;
				cachedMet = predicate();
				return cachedMet;
			}

			return cachedMet;
		}

		public virtual UIInfoElement ProvideUIInfoElement()
		{
			checklistInfoElement.State = IsMet();
			UIInfoElement infoElement = checklistInfoElement.ProvideUI();
			infoElement.Activate();
			infoElement.SetTextLeft(55, 0);
			infoElement.Height = new(57f, 0f);
			return infoElement;
		}

		public virtual UIElement ProvideUIInfoElement(ChecklistInfoElement.ExtraIconType notMetIcon = ChecklistInfoElement.ExtraIconType.CrossmarkRed, ChecklistInfoElement.ExtraIconType metIcon = ChecklistInfoElement.ExtraIconType.CheckmarkGreen)
		{
			checklistInfoElement.NotMetIcon = notMetIcon;
			checklistInfoElement.MetIcon = metIcon;
			return ProvideUIInfoElement();
		}
	}
}
