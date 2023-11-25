using Macrocosm.Common.UI;
using System;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistCondition
    {
        public bool HideIfMet { get; set; } = true;
        public string LangKey { get; private set; }

        public bool HasChanged => hasChanged;
        public bool IsMet => cachedMet;

        private readonly Func<bool> predicate = () => false;

        private bool lastMet;
        private bool hasChanged;
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

        public ChecklistCondition(string langKey, string customIconMet, string customIconNotMet, Func<bool> canLaunch, int checkPeriod = 1, bool hideIfMet = false)
        {
            LangKey = langKey;
            predicate = canLaunch;
            this.checkPeriod = checkPeriod;
            HideIfMet = hideIfMet;

            checklistInfoElement = new(langKey, customIconMet, customIconNotMet);
        }

        public bool Check()
        {
            checkCounter++;

            if (checkCounter >= checkPeriod)
            {
                checkCounter = 0;

                bool currentMet = predicate();
                if (currentMet != lastMet)
                {
                    hasChanged = true;
                    cachedMet = currentMet;
                }
                else
                {
                    hasChanged = false;
                }

                lastMet = currentMet;
            }

            return cachedMet;
        }

        public virtual UIInfoElement ProvideUIInfoElement()
        {
            checklistInfoElement.MetState = cachedMet;
            UIInfoElement infoElement = checklistInfoElement.ProvideUI();
            infoElement.Activate();
            infoElement.SetTextLeft(50, 0);
            infoElement.Height = new(52f, 0f);
            infoElement.IconHAlign = 0.12f;
            return infoElement;
        }
    }
}
