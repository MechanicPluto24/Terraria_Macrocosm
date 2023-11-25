using Macrocosm.Common.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistInfoElement : InfoElement
    {
        public bool MetState { get; set; } = false;

        private string iconMet;
        private string iconNotMet;

        public ChecklistInfoElement(string langKey) : base(langKey)
        {
            this.iconMet = "Checklist/" + langKey + "_Met";
            this.iconNotMet = "Checklist/" + langKey + "_NotMet";
        }

        public ChecklistInfoElement(string langKey, string customIconMet, string customIconNotMet) : base(langKey)
        {
            this.iconMet = customIconMet;
            this.iconNotMet = customIconNotMet;
        }

        protected override Asset<Texture2D> GetIcon() => ModContent.RequestIfExists<Texture2D>("Macrocosm/Content/Rockets/Textures/" + (MetState ? iconMet : iconNotMet), out var texture) ? texture : null;

        protected override Asset<Texture2D> GetIconSymbol() => null;

        private string KeySelector => specialValueKey + "." + (MetState ? "Met" : "NotMet") + ".";

        protected override LocalizedColorScaleText GetText()
        {
            string key = "Mods.Macrocosm.UI.Rocket.Checklist." + KeySelector + "Display";
            LocalizedText text = Language.GetText(key);

            if (text.Value is "" or "default" || text.Value == key)
                return new(LocalizedText.Empty);
            else
                return new(text);
        }

        protected override LocalizedText GetHoverText()
        {
            string key = "Mods.Macrocosm.UI.Rocket.Checklist." + KeySelector + "Hover";
            LocalizedText hoverText = Language.GetText(key);

            if (hoverText.Value is "" or "default" || hoverText.Value == key)
                return LocalizedText.Empty;
            else
                return hoverText;
        }
    }
}
