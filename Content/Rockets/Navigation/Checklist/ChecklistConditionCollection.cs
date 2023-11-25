using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistConditionCollection : IEnumerable<ChecklistCondition>
    {
        private List<ChecklistCondition> conditions = new();

        public ChecklistCondition this[int index] => conditions[index];

        public void Add(ChecklistCondition condition)
            => conditions.Add(condition);

        public void AddRange(List<ChecklistCondition> conditions)
            => this.conditions.AddRange(conditions);

        public void AddRange(ChecklistConditionCollection conditions)
            => this.conditions.AddRange(conditions.ToList());

        public void Remove(ChecklistCondition condition)
             => conditions.Remove(condition);

        public void Remove(string key)
             => Remove(conditions.Find(x => x.LangKey == key));

        public void Append(ChecklistConditionCollection extraConditions)
            => conditions.AddRange(extraConditions);

        public static ChecklistConditionCollection Merge(ChecklistConditionCollection conditions1, ChecklistConditionCollection conditions2)
        {
            ChecklistConditionCollection output = new();
            output.Append(conditions1);
            output.Append(conditions2);

            return output;
        }

        public bool AllMet()
        {
            foreach (var condition in conditions)
            {
                if (!condition.Check())
                    return false;
            }

            return true;
        }

        public List<UIElement> ProvideUIElementList()
        {
            var list = new List<UIElement>();

            foreach (var condition in conditions)
            {
                // If true, add only if not hidden while true
                if (!condition.HideIfMet || !condition.Check())
                    list.Add(condition.ProvideUIInfoElement());
            }

            return list;
        }

        public void AddToUI(UIElement element)
        {
            foreach (var condition in conditions)
            {
                // If true, add only if not hidden while true
                if (!condition.HideIfMet || !condition.Check())
                    element.Append(condition.ProvideUIInfoElement());
            }
        }

        public IEnumerator<ChecklistCondition> GetEnumerator()
            => conditions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => conditions.GetEnumerator();
    }
}
