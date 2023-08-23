

using Terraria.UI;
using Microsoft.Xna.Framework.Input;
using System;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Macrocosm.Common.UI;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
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
			formattedLocalizedText = Language.GetText("Mods.Macrocosm.UI.Rocket.ThreatLevel.Threat" + (int)value).WithFormatArgs((int)value);
            this.textColor = textColor; 
		}

        protected override LocalizedColorScaleText GetText() => new(formattedLocalizedText, textColor, scale: 0.9f);

        protected override LocalizedText GetHoverText() => Language.GetText("Mods.Macrocosm.UI.Rocket.ThreatLevel.Name");

        protected override Asset<Texture2D> GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Icons/ThreatLevel");

	}
}
