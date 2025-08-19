using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class MoonNightBiome : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public override float GetWeight(Player player) => 0.6f;


    public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/MoonNight";
    public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/MoonNight";
    public override string MapBackground => BackgroundPath;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

    public override bool IsBiomeActive(Player player) => SubworldSystem.IsActive<Moon>() && !Main.dayTime;
}
