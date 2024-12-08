using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Macrocosm.Content.Subworlds
{
    public class EarthOrbitSubworld : OrbitSubworld
    {
        protected override int InstanceCount => 10;
        public override string ParentSubworldID => Earth.ID;

        public override void OnEnterSubworld()
        {
            SkyManager.Instance.Activate("Macrocosm:EarthOrbitSky");
        }

        public override void OnExitSubworld()
        {
            SkyManager.Instance.Deactivate("Macrocosm:EarthOrbitSky");
        }

        public override float GravityMultiplier => 0f;
        public override float AtmosphericDensity => 0.1f;
        public override int[] EvaporatingLiquidTypes => [LiquidID.Water];
        public override float GetAmbientTemperature() => Utility.ScaleNoonToMidnight(-65f, 125f);
        public override Dictionary<MapColorType, Color> MapColors => new()
        {
            {MapColorType.SkyUpper, Color.Black},
            {MapColorType.SkyLower, Color.Black},
            {MapColorType.UndergroundUpper, Color.Black},
            {MapColorType.UndergroundLower, Color.Black},
            {MapColorType.CavernUpper, Color.Black},
            {MapColorType.CavernLower, Color.Black},
            {MapColorType.Underworld, Color.Black}
        };

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ)
            {
                if (!SkyManager.Instance["Macrocosm:EarthOrbitSky"].IsActive())
                    SkyManager.Instance.Activate("Macrocosm:EarthOrbitSky");
            }
        }
    }
}
