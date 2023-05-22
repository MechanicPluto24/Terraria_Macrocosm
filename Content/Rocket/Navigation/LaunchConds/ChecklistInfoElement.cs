using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm.Content.Rocket.Navigation.InfoElements;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConds
{
    public class ChecklistInfoElement : BasicInfoElement
    {
        public enum IconType
        {
            Checkmark,
            Crossmark,
            GrayCrossmark,
            QuestionMark
        }

        public bool State { get; set; } = false;

        public IconType TrueIcon { get; set; } = IconType.Checkmark;
        public IconType FalseIcon { get; set; } = IconType.Crossmark;

		public ChecklistInfoElement(string langKey) : base(langKey)
        {
        }

        protected override Texture2D GetIcon()
        {
            if (State)
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/Navigation/Icons/" + TrueIcon.ToString()).Value;
            else
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/Navigation/Icons/" + FalseIcon.ToString()).Value;
        }

        private string KeySelector => specialValueLangKey + "." + (State ? "True" : "False") + ".";
        protected override string GetText() => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist." + KeySelector + "Display");

		protected override string GetHoverText()
        {
            string key = "Mods.Macrocosm.WorldInfo.Checklist." + KeySelector + "Hover";
			string hoverText = Language.GetTextValue(key);

            if (hoverText is "" or "default" || hoverText == key)
                return "";
            else
                return hoverText;
	    }
    }
}
