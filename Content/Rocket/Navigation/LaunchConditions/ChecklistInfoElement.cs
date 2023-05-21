using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm.Content.Rocket.Navigation.InfoElements;

namespace Macrocosm.Content.Rocket.Navigation.LaunchConditions
{
    public class ChecklistInfoElement : BasicInfoElement
    {
        public bool State { get; set; } = false;

        public ChecklistInfoElement(string langKey, bool defaultState = false) : base(langKey) 
        {
            State = defaultState;
        }

        protected override Texture2D GetIcon()
        {
            if (State)
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/NavigationUI/Icons/Checkmark").Value;
            else
                return ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/NavigationUI/Icons/Crossmark").Value;
        }

        private string KeySelector => State ? ".Good" : ".Bad";
        protected override string GetText() => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist." + specialValueLangKey + KeySelector);
        protected override string HoverText => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist." + specialValueLangKey + KeySelector + "Hover");
    }
}
