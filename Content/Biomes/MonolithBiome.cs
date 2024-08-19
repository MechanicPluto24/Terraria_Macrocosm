using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Players;
namespace Macrocosm.Content.Biomes
{
    public class MonolithBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;


        public override int Music => 0;

        public override void SetStaticDefaults()
        {
        }

        public override void OnInBiome(Player player)
        {
            base.OnInBiome(player);
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
        }

        public override bool IsBiomeActive(Player player)
            => TileCounts.Instance.MonolithCount > 0;

    }
}
