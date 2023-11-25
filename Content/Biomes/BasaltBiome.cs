using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class BasaltBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override string BestiaryIcon => "Macrocosm/Content/Biomes/MoonBiome_Icon";
        public override string BackgroundPath => "Macrocosm/Content/Biomes/MoonBiome_Background";
        public override string MapBackground => BackgroundPath;

        //public override Color? BackgroundColor => base.BackgroundColor;
        //public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBgStyle>();
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUgBgStyle>();
        //public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

        public override void SetStaticDefaults()
        {
        }

        public override void OnInBiome(Player player)
        {
            base.OnInBiome(player);
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
        }

        public override bool IsBiomeActive(Player player)
            => false; // TileCounts.Instance.BasaltCount > 40;

    }
}


