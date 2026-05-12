using Macrocosm.Common.Events;
using Macrocosm.Content.Backgrounds.Moon;
using Macrocosm.Content.Events;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class DemonSunBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/BloodMoon";
    public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";
    public override string MapBackground => BackgroundPath;

    public override Color? BackgroundColor => base.BackgroundColor;
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();

    public override void SetStaticDefaults()
    {
    }

    public override bool IsBiomeActive(Player player) => SubworldSystem.IsActive<Moon>() && MacrocosmEventSystem.IsActive<DemonSunEvent>();

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
