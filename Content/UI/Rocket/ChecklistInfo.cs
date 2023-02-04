using Terraria.Localization;
using System.Collections.Generic;

namespace Macrocosm.Content.UI.Rocket
{
    public class ChecklistInfo
	{
		public ChecklistInfoElement Fuel = new(ChecklistItemType.Fuel);
		public ChecklistInfoElement Destination = new(ChecklistItemType.Destination);
		public ChecklistInfoElement Obstruction = new(ChecklistItemType.Obstruction);

		private List<BasicInfoElement> elements;

        public void Add(BasicInfoElement element) { elements.Add(element); }

        public ChecklistInfo() 
        {
			elements = new()
			{
				Fuel,
				Destination,
				Obstruction
			};
		}

        public UIFlightChecklist ProvideUI()
        {
			UIFlightChecklist panel = new(Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist.DisplayName"));

            if (elements is not null)
                 foreach (BasicInfoElement element in elements)
                     panel.Add(element.ProvideUI());
  
            return panel;
        }
    }
}
