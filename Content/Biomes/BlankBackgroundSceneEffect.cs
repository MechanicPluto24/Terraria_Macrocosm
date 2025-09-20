using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Backgrounds.Blank;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class BlankBackgroundSceneEffect : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;
    public override float GetWeight(Player player) => 1f;

    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<BlankSufraceBackgroundStyle>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<BlankUndergroundBackgroundStyle>();

    public override bool IsSceneEffectActive(Player player) => SubworldSystem.Current is MacrocosmSubworld subworld && subworld.NoBackground;
}
