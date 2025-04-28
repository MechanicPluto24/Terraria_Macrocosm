using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI.Rockets.Navigation.Info
{
    public enum ThreatLevel
    {
        None,
        Harmless,
        Arduous,
        Challenging,
        Dangerous,
        Treacherous,
        Lethal,
        Cataclysmic,
        Nightmare,
        Apollyon,
        Unknown
    }

    public class ThreatLevelInfoElement : InfoElement
    {

        private LocalizedText formattedLocalizedText;

        private Color textColor;

        public ThreatLevelInfoElement(ThreatLevel level, Color textColor) : base((float)level)
        {
            formattedLocalizedText = Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.ThreatLevel.Threat" + (int)value).WithFormatArgs((int)value);
            this.textColor = textColor;
        }

        protected override LocalizedColorScaleText GetText() => new(formattedLocalizedText, textColor, scale: 0.9f);

        protected override LocalizedText GetHoverText() => Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.ThreatLevel.Name");

        protected override Asset<Texture2D> GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Rockets/WorldInfo/ThreatLevel");

    }
}
