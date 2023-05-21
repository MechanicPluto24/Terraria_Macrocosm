using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
{
    public class HazardInfoElement : BasicInfoElement
    {
        public HazardInfoElement(string hazardKey) : base(hazardKey) { }

        protected override Texture2D GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rocket/NavigationUI/Icons/" + specialValueLangKey, AssetRequestMode.ImmediateLoad).Value;

        protected override string GetText() => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Hazard." + specialValueLangKey);

        protected override string HoverText => Language.GetTextValue("Mods.Macrocosm.WorldInfo.Hazard.Name");
    }
}
