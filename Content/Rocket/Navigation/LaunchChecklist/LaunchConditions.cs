using Terraria.UI;
using Terraria.Localization;
using System.Collections.Generic;
using System.Collections;

namespace Macrocosm.Content.Rocket.Navigation.LaunchChecklist
{
    public class LaunchConditions : IEnumerable<ChecklistCondition>
    {
		private List<ChecklistCondition> conditions = new();

        public ChecklistCondition this[int index] => conditions[index];

		public void Add(ChecklistCondition condition) 
            => conditions.Add(condition); 

		public void Remove(ChecklistCondition condition)
			 => conditions.Remove(condition);

        public void Remove(string key)
             => Remove(conditions.Find(x => x.LangKey == key));

        public void Append(LaunchConditions extraConditions)
            => conditions.AddRange(extraConditions);

        public static LaunchConditions Merge(LaunchConditions conditions1, LaunchConditions conditions2)
        {
            LaunchConditions output = new();
            output.Append(conditions1);
            output.Append(conditions2);

            return output;
        }

        public bool Check()
        {
            foreach (var condition in conditions) 
            {
                if(!condition.IsMet())
                    return false;
            }

            return true;
        }

        public List<UIElement> ProvideList()
        {
            var list = new List<UIElement>();

            foreach(var condition in conditions)
            {
                // If true, add only if not hidden while true
                if(!condition.HideIfTrue || !condition.IsMet())
                    list.Add(condition.ProvideUI());
            }

            return list;
        }

        public void AddToUI(UIElement element)
        {
            foreach (var condition in conditions) 
            {
				// If true, add only if not hidden while true
				if (!condition.HideIfTrue || !condition.IsMet())
					element.Append(condition.ProvideUI());
            }
        }

		public IEnumerator<ChecklistCondition> GetEnumerator()
		    => conditions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		    => conditions.GetEnumerator();
	}
}
