using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Macrocosm.Content.UI.Rocket
{
	public enum ChecklistItemType
	{
		Fuel,
		Destination,
		Obstruction
	}

	public class ChecklistInfoElement : BasicInfoElement
    {
		public bool State { get; set; } = false;

		public ChecklistInfoElement(string langKey) : base(langKey) { }
		public ChecklistInfoElement(ChecklistItemType type) : base(typeToKey[(int)type]) { }

        protected override Texture2D GetIcon()
        {
			if (State)
				return ModContent.Request<Texture2D>("Macrocosm/Content/UI/Rocket/Icons/Checkmark").Value;
			else
				return ModContent.Request<Texture2D>("Macrocosm/Content/UI/Rocket/Icons/Crossmark").Value;
		}

		private string KeySelector => State ? ".Good" : ".Bad";
		protected override string GetText() => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist." + specialValueLangKey + KeySelector);

        protected override string HoverText => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist." + specialValueLangKey + KeySelector + "Hover");


		private static string[] typeToKey = {
			"Fuel",
			"Destination",
			"Obstruction"
		};
	}
}
