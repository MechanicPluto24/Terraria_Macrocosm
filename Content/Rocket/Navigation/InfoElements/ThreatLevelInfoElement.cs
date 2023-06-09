

using Terraria.UI;
using Microsoft.Xna.Framework.Input;
using System;
using System.Globalization;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Utils;
using log4net.Core;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
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

    public class ThreatLevelInfoElement : BasicInfoElement
    {
        public ThreatLevelInfoElement(ThreatLevel level) : base((float)level) { }
        public ThreatLevelInfoElement(int level) : base(level) { }

        protected override string GetHoverText() => Language.GetTextValue("Mods.Macrocosm.WorldInfo.ThreatLevel.Name");

        protected override Texture2D GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/Navigation/Icons/ThreatLevel", AssetRequestMode.ImmediateLoad).Value;

        protected override string GetText()
        {
            string threatLevel = ((int)value).ToString();
            string threatLevelName = Language.GetTextValue("Mods.Macrocosm.WorldInfo.ThreatLevel.Threat" + ((int)value).ToString());

            return threatLevel + " - " + threatLevelName;
        }

        protected override Color TextColor => GetTextColor();
        private Color GetTextColor()
        {
            if (value <= 3)
                return Color.White;
            else if (value <= 6)
                return Color.Yellow;
            else if (value <= 9)
                return Color.Red;
            else
                return Color.Purple;
        }
    }
}
