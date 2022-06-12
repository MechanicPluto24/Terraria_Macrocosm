using Terraria;

namespace Macrocosm.Common.Utility {
    public static class MathFHelper {
        // Thank you code from TerrariaAmbience
        public static double CreateGradientValue(double value, double min, double max) {
            double mid = (max + min) / 2;
            double returnValue;

            if (value > mid) {
                var thing = 1f - (value - min) / (max - min) * 2;
                returnValue = 1f + thing;
                return returnValue;
            }
            returnValue = (value - min) / (max - min) * 2;
            returnValue = Utils.Clamp(returnValue, 0, 1);
            return returnValue;
        }
        public static float CreateGradientValue(float value, float min, float max) {
            float mid = (max + min) / 2;
            float returnValue;

            if (value > mid) {
                var thing = 1f - (value - min) / (max - min) * 2;
                returnValue = 1f + thing;
                return returnValue;
            }
            returnValue = (value - min) / (max - min) * 2;
            returnValue = Utils.Clamp(returnValue, 0, 1);
            return returnValue;
        }
    }
}