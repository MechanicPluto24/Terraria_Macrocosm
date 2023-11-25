using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
    public class CelestialDisco : ModSystem
    {
        public enum CelestialType { Nebula, Stardust, Vortex, Solar }
        public static CelestialType CelestialStyle { get; set; } = CelestialType.Nebula;
        public static CelestialType NextCelestialStyle
            => CelestialStyle == CelestialType.Solar ? CelestialType.Nebula : CelestialStyle + 1;

        public static Color NebulaColor { get; set; } = new(165, 0, 204);
        public static Color StardustColor { get; set; } = new(0, 187, 255);
        public static Color VortexColor { get; set; } = new(0, 255, 180);
        public static Color SolarColor { get; set; } = new(255, 191, 0);

        public static Color CelestialColor { get; set; }

        public static float CelestialStyleProgress;
        private static int celesialCounter = 0;
        private static readonly Color[] celestialColors = { NebulaColor, StardustColor, VortexColor, SolarColor };

        public override void PostUpdateEverything()
        {
            UpdateCelestialStyle();
        }

        private static void UpdateCelestialStyle()
        {
            float cyclePeriod = 90f;
            if (celesialCounter++ >= (int)cyclePeriod)
            {
                celesialCounter = 0;
                CelestialStyle = NextCelestialStyle;
            }

            CelestialStyleProgress = celesialCounter / cyclePeriod;

            CelestialColor = Color.Lerp(celestialColors[(int)CelestialStyle], celestialColors[(int)NextCelestialStyle], CelestialStyleProgress);
        }
    }
}
