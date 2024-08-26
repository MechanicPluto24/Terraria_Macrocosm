using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Biomes
{
    public class MonolithBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => base.Priority + 3;
        public override int Music => 0;

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => base.IsBiomeActive(player) && TileCounts.Instance.MonolithCount > 0;

        public override void OnInBiome(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
