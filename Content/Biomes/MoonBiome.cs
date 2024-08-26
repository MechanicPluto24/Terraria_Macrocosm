using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class MoonBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Moon";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";
        public override string MapBackground => BackgroundPath;

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player)
            => SubworldSystem.IsActive<Moon>();

        public override void OnInBiome(Player player)
        {
        }

        public override void OnEnter(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
    public class NightMoonBiome : ModBiome//Really just exists for icon and BG purposes, if you know a better way to do so than inform me...
    {
        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/NightMoon";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player)
            => false;

        public override void OnInBiome(Player player)
        {
        }

        public override void OnEnter(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
