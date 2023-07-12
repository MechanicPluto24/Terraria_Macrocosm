using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm.Common.UI;
using ReLogic.Content;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistInfoElement : InfoElement
    {
        public enum IconType
        {
            Checkmark,
            Crossmark,
            GrayCrossmark,
            QuestionMark
        }

        public bool State { get; set; } = false;

        public IconType MetIcon { get; set; } = IconType.Checkmark;
        public IconType NotMetIcon { get; set; } = IconType.Crossmark;

		public ChecklistInfoElement(string langKey) : base(langKey)
        {
        }

        protected override Asset<Texture2D> GetIcon()
        {
            if (State)
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Icons/" + MetIcon.ToString());
            else
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Icons/" + NotMetIcon.ToString());
        }

        private string KeySelector => specialValueKey + "." + (State ? "True" : "False") + ".";

        protected override LocalizedColorScaleText GetText() => new(Language.GetText("Mods.Macrocosm.RocketUI.Checklist." + KeySelector + "Display"), scale: 1.2f);

		protected override LocalizedText GetHoverText()
        {
            string key = "Mods.Macrocosm.RocketUI.Checklist." + KeySelector + "Hover";
			LocalizedText hoverText = Language.GetText(key);

            if (hoverText.Value is "" or "default" || hoverText.Value == key)
                return LocalizedText.Empty;
            else
                return hoverText;
	    }
    }
}
