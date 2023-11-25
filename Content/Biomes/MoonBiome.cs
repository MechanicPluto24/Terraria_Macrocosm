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
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

        public override string BestiaryIcon => Macrocosm.TextureAssetsPath + "Icons/Moon";
        public override string BackgroundPath => Macrocosm.TextureAssetsPath + "MapBackgrounds/Moon";
        public override string MapBackground => BackgroundPath;

        public override void SetStaticDefaults()
        {
        }

        public override void OnInBiome(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }

        public override bool IsBiomeActive(Player player)
            => SubworldSystem.IsActive<Moon>();

    }
}
