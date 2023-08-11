using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm.Common.UI;
using ReLogic.Content;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistInfoElement : InfoElement
    {
        public enum ExtraIconType
        {
            GreenCheckmark,
            RedCrossmark,
            GrayCrossmark,
            GoldQuestionMark
        }

        public bool State { get; set; } = false;

        public ExtraIconType MetIcon { get; set; } = ExtraIconType.GreenCheckmark;
        public ExtraIconType NotMetIcon { get; set; } = ExtraIconType.RedCrossmark;

		public ChecklistInfoElement(string langKey) : base(langKey)
        {
        }

        protected override Asset<Texture2D> GetExtraIcon()
        {
            if (State)
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Symbols/" + MetIcon.ToString());
            else
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Symbols/" + NotMetIcon.ToString());
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
