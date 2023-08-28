using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Macrocosm.Common.UI;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    internal class HazardInfoElement : InfoElement
    {
        public HazardInfoElement(string hazardKey) : base(hazardKey) { }

        protected override Asset<Texture2D> GetIcon() => ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Icons/" + specialValueKey);

        protected override LocalizedColorScaleText GetText() => new(Language.GetText("Mods.Macrocosm.UI.Rocket.Hazard." + specialValueKey), scale: 0.9f);

        protected override LocalizedText GetHoverText() => Language.GetText("Mods.Macrocosm.UI.Rocket.Hazard.Name");
    }
}
