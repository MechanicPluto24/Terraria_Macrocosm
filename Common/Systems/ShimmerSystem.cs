using Macrocosm.Common.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class ShimmerSystem : ModSystem
    {
        private record ShimmerOverride(Condition Condition, int ResultType);

        private static readonly Dictionary<int, List<ShimmerOverride>> itemShimmerOverrides = new();

        public override void Load()
        {
            On_Item.GetShimmered += On_Item_GetShimmered;
        }

        public override void Unload()
        {
            On_Item.GetShimmered -= On_Item_GetShimmered;
        }

        public override void PostSetupContent()
        {
            // Register vanilla to vanilla (or other mod) shimmer overrides here
            // Macrocosm items should do it in their SetStaticDefaults methods
        }

        public static void RegisterOverride(int itemType, int resultType, Condition condition = null)
        {
            if (ItemID.Sets.ShimmerTransformToItem[itemType] <= 0)
                ItemID.Sets.ShimmerTransformToItem[itemType] = resultType;

            if (!itemShimmerOverrides.TryGetValue(itemType, out _))
                itemShimmerOverrides[itemType] = new();

            itemShimmerOverrides[itemType].Add(new ShimmerOverride(condition, resultType));
        }

        private void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self)
        {
            if (itemShimmerOverrides.TryGetValue(self.type, out List<ShimmerOverride> shimmerOverrides))
            {
                foreach (ShimmerOverride shimmerOverride in shimmerOverrides)
                {
                    if (shimmerOverride.Condition is null || shimmerOverride.Condition.Predicate.Invoke())
                    {
                        self.Shimmer(shimmerOverride.ResultType);
                        return;
                    }
                }

                orig(self);
            }
        }
    }
}
