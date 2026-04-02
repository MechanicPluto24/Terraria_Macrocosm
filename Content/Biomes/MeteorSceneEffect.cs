using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Systems.Flags;

namespace Macrocosm.Content.Biomes;

public class MeteorSceneEffect : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CollisionCourse");
    public override bool IsSceneEffectActive(Player player) => WorldData.Current.MeteorStorm;
}
