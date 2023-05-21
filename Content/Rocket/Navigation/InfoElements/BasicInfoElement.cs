using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
{
    public class BasicInfoElement
    {
        protected float value = float.MinValue;
        protected string specialValueLangKey = "default";

        public bool HasValue => value != float.MinValue;
        public bool HasSpecial => specialValueLangKey != "default";

        protected BasicInfoElement(string specialValueLangKey)
        {
            this.specialValueLangKey = specialValueLangKey;
        }

        protected BasicInfoElement(float value)
        {
            this.value = value;
        }

        protected BasicInfoElement(float value, string specialValueLangKey)
        {
            this.value = value;
            this.specialValueLangKey = specialValueLangKey;
        }

        protected virtual Texture2D GetIcon() => null;
        protected virtual string GetText() => Language.GetTextValue(specialValueLangKey);
        protected virtual string HoverText => "";
        protected virtual string Units => "";
        protected virtual Color TextColor => Color.White;

        public virtual UIElement ProvideUI()
        {
            if (!HasValue && !HasSpecial)
                return null;

            Texture2D icon = GetIcon() ?? Macrocosm.EmptyTex;

            return new UIInfoElement(icon, GetText(), HoverText, Units, TextColor);
        }
    }
}
