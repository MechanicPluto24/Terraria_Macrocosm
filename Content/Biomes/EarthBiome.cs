using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    /// <summary> Only used in the bestiary </summary>
    public class EarthBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;
        public override float GetWeight(Player player) => 0f;

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Earth";

        // To avoid MissingResourceException and defaulting in the bestiary
        public override string BackgroundPath => null;
        public override string MapBackground => null;

        public override bool IsBiomeActive(Player player) => false;
    }
}
