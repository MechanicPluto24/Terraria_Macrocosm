using Macrocosm.Common.Systems;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Biomes
{
    public class MonolithBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override int Music => 0;

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => TileCounts.Instance.MonolithCount > 0;

        public override void OnInBiome(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
