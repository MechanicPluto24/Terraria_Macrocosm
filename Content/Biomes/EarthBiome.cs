using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    /// <summary> Only used in the bestiary </summary>
    public class EarthBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;
        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Earth";

        // To avoid MissingResourceException and defaulting in the bestiary
        public override string BackgroundPath => null;
        public override string MapBackground => null;

        public override bool IsBiomeActive(Player player) => !SubworldSystem.AnyActive<Macrocosm>();
    }
}
