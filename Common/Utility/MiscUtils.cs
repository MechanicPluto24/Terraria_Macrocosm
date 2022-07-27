using Terraria;

namespace Macrocosm.Common.Utility {
    public static class MiscUtils {
        /// <summary>
        /// Hostile projectiles deal 2x the <paramref name="damage"/> in Normal Mode and 4x the <paramref name="damage"/> in Expert Mode.
        /// This helper method remedies that.
        /// </summary>
        public static int TrueDamage(int damage)
            => damage / (Main.expertMode ? 4 : 2);

        /// <summary>
        /// Applies a logarithmic derivative to <paramref name="value"/>
        /// </summary>
        /// <param name="value">The initial value</param>
        /// <param name="targetValue">The target value</param>
        /// <param name="scaleFactor">The factor (commonly notated as <c>k</c>) which affects how quickly or slowly <paramref name="value"/> reaches <paramref name="targetValue"/></param>
        /// <param name="deltaTime">The change in time</param>
        /// <returns>The updated value</returns>
        public static float ScaleLogarithmic(float value, float targetValue, float scaleFactor, float deltaTime) {
            float dt = scaleFactor * (targetValue - value) * deltaTime;
            value += dt;
            return value;
        }
    }
}
