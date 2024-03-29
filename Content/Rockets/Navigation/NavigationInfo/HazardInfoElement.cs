using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    public class HazardInfoElement : InfoElement
    {
        public HazardInfoElement(string hazardKey) : base(hazardKey) { }

        protected override Asset<Texture2D> GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/WorldInfo/" + specialValueKey);

        protected override LocalizedColorScaleText GetText() => new(Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Hazard." + specialValueKey, () => Utility.SanitizeCodeCase(specialValueKey)), scale: 0.9f);

        protected override LocalizedText GetHoverText() => Language.GetText("Mods.Macrocosm.UI.Rocket.Hazard.Name");
    }
}
