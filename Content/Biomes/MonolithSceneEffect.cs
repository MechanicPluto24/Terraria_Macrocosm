using Macrocosm.Common.Systems;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Biomes
{
    public class MonolithBiome : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override float GetWeight(Player player) => 1f;
        public override int Music => 0;
        public override bool IsSceneEffectActive(Player player) => TileCounts.Instance.MonolithCount > 0;
    }
}
