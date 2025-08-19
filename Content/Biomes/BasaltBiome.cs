using Macrocosm.Content.Backgrounds.Moon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class BasaltBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Moon";
    public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";
    public override string MapBackground => BackgroundPath;

    public override Color? BackgroundColor => base.BackgroundColor;
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();

    //public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/BasaltBiomeTheme");

    public override void SetStaticDefaults()
    {
    }

    public override bool IsBiomeActive(Player player) => false; /*SubworldSystem.IsActive<Moon>() && TileCounts.Instance.BasaltRockCount > 400*/

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


