﻿using System;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation.LaunchChecklist
{
	public class ChecklistCondition
	{
		public string LangKey { get; private set; }

		private readonly Func<bool> predicate = () => false;

		public bool HideIfTrue = true;

		private readonly ChecklistInfoElement checklistInfoElement;

		public ChecklistCondition(string langKey, Func<bool> canLaunch, bool hideIfTrue = false)
		{
			LangKey = langKey;
			predicate = canLaunch;
			HideIfTrue = hideIfTrue;

			checklistInfoElement = new(langKey);
		}

		public virtual bool IsMet() => predicate();

		public virtual UIElement ProvideUI()
		{
			checklistInfoElement.State = IsMet();
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