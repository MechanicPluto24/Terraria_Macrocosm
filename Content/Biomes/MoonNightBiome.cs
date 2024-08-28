using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class MoonNightBiome : ModBiome 
    {
        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/MoonNight";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/MoonNight";
        public override string MapBackground => BackgroundPath;
        public override bool IsBiomeActive(Player player) => SubworldSystem.IsActive<Moon>() && !Main.dayTime;
    }
}
