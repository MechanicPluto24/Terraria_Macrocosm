using Terraria.UI;
using Terraria.Localization;
using System.Collections.Generic;
using System.Collections;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConds
{
    public class LaunchConditions : IEnumerable<LaunchCondition>
    {
		public List<LaunchCondition> conditions = new();

        public LaunchCondition this[int index] => conditions[index];

		public void Add(LaunchCondition condition) 
            => conditions.Add(condition); 

		public void Remove(LaunchCondition condition)
			 => conditions.Remove(condition);

        public void Remove(string key)
             => Remove(conditions.Find(x => x.LangKey == key));

        public void Merge(LaunchConditions extraConditions)
            => conditions.AddRange(extraConditions);

        public bool Check()
        {
            foreach (var condition in conditions) 
            {
                if(!condition.Check())
                    return false;
            }

            return true;
        }

        public List<UIElement> ProvideList()
        {
            var list = new List<UIElement>();

            foreach(var condition in conditions)
                list.Add(condition.ProvideUI());

            return list;
        }

        public void AddToUI(UIElement element)
        {
            foreach (var condition in conditions) 
            {
                element.Append(condition.ProvideUI());
            }
        }

		public IEnumerator<LaunchCondition> GetEnumerator()
		    => conditions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		    => conditions.GetEnumerator();
	}
}
