using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Biomes
{
    public class MonolithBiome : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Silence");
        public override bool IsSceneEffectActive(Player player) => TileCounts.Instance.EnoughTilesForMonolith;
    }
}
