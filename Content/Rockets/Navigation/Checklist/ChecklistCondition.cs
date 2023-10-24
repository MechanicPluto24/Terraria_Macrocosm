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

		public ChecklistCondition(string langKey, Func<bool> canLaunch, int checkPeriod = 1, bool hideIfMet = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			this.checkPeriod = checkPeriod;
			HideIfMet = hideIfMet;

			checklistInfoElement = new(langKey);
		}

		public ChecklistCondition(string langKey, string iconMet, string iconNotMet, Func<bool> canLaunch, int checkPeriod = 1,  bool hideIfMet = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			this.checkPeriod = checkPeriod;
			HideIfMet = hideIfMet;

			checklistInfoElement = new(langKey, iconMet, iconNotMet);
		}

		public ChecklistCondition(string langKey, string uniqueIcon, Func<bool> canLaunch, int checkPeriod = 1, bool hideIfMet = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			this.checkPeriod = checkPeriod;
			HideIfMet = hideIfMet;

			checklistInfoElement = new(langKey, uniqueIcon);
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
			checklistInfoElement.MetState = IsMet();
			UIInfoElement infoElement = checklistInfoElement.ProvideUI();
			infoElement.Activate();
			infoElement.SetTextLeft(50, 0);
			infoElement.Height = new(52f, 0f);
			infoElement.IconHAlign = 0.12f;
			return infoElement;
		}
	}
}
