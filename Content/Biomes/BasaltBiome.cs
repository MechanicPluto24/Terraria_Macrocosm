using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes
{
    public class BasaltBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

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

        public override bool IsBiomeActive(Player player)
            => false; // TileCounts.Instance.BasaltCount > 40;

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


