using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace Macrocosm.Common.UI
{
    public class InfoElement
    {
        protected float value = float.MinValue;
        protected string specialValueKey = "default";

        public bool HasValue => value != float.MinValue;
        public bool HasSpecial => specialValueKey != "default";


        public InfoElement(string specialValueKey)
        {
            this.specialValueKey = specialValueKey;
        }

        public InfoElement(float value, string specialValueKey = "")
        {
            this.value = value;
            this.specialValueKey = specialValueKey;
        }

        protected virtual Asset<Texture2D> GetIcon() => null;
        protected virtual Asset<Texture2D> GetIconSymbol() => null;
        protected virtual LocalizedColorScaleText GetText() => new(Language.GetText(specialValueKey));
        protected virtual LocalizedText GetHoverText() => LocalizedText.Empty;

        public virtual UIInfoElement ProvideUI()
        {
            if (!HasValue && !HasSpecial)
                return null;

            return new UIInfoElement(GetText(), GetIcon(), GetIconSymbol(), GetHoverText());
        }
    }
}
