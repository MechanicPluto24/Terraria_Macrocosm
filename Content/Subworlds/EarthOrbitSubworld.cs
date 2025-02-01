using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Skies.EarthOrbit;
using Macrocosm.Content.Skies.Moon;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Utilities;

namespace Macrocosm.Content.Subworlds
{
    public partial class EarthOrbitSubworld : OrbitSubworld
    {
        protected override int InstanceCount => 50;
        public override string ParentSubworldID => Earth.ID;

        public override bool PeacefulWorld => true;

        public override string CustomSky => nameof(EarthOrbitSky);
        protected override float GravityMultiplier => 0f;
        public override int[] EvaporatingLiquidTypes => [LiquidID.Water];

        protected override float AtmosphericDensity(Vector2 position) => 0.1f;
        //public override float AmbientTemperature(Vector2 position) => Utility.ScaleNoonToMidnight(-65f, 125f); why is this not working
        public override WorldSize GetSubworldSize(WorldSize earthWorldSize) => new(1600, 1200);

        public override bool NoBackground => true;
        
        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            Utility.ApplySurfaceLight(tile, x, y, ref color);
            if(color.X==0f)
                color.X = 0.004f;
            if(color.Y==0f)
                color.Y = 0.004f;
            if(color.Z==0f)
                color.Z = 0.004f;
            return false;//THIS MUST RETURN FALSE -Clyder8
        }
        
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

        public override void PostLoad()
        {
            WorldFlags.SubworldUnlocked.SetValue(ID, false);
        }
    }
}
