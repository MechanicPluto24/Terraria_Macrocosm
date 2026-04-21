using Macrocosm.Common.Events;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Events;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Biomes;

public class MeteorSceneEffect : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CollisionCourse");
    public override bool IsSceneEffectActive(Player player) => MacrocosmEventSystem.IsActive<MeteorStormEvent>();
}
