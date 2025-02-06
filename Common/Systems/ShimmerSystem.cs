using Macrocosm.Common.Systems.Flags;
using Macrocosm.Content.Items.Blocks.Bricks;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
            RegisterOverride(ItemID.LunarBrick, new Condition(Condition.BloodMoon.Description, () => Main.bloodMoon || WorldData.DemonSun), ModContent.ItemType<HaemonovaBrick>());
        }

        public static void RegisterOverride(int itemType, Condition condition, int resultType)
        {
            if (ItemID.Sets.ShimmerTransformToItem[itemType] <= 0)
                ItemID.Sets.ShimmerTransformToItem[itemType] = resultType;

            if (!itemShimmerOverrides.TryGetValue(itemType, out _))
                itemShimmerOverrides[itemType] = new();

            itemShimmerOverrides[itemType].Add(new ShimmerOverride(condition, resultType));
        }

        private void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self)
        {
            if (itemShimmerOverrides.TryGetValue(self.type, out var overrides))
            {
                foreach (var shimmerOverride in overrides)
                {
                    if (shimmerOverride.Condition.Predicate.Invoke())
                    {
                        int originalStack = self.stack;

                        self.SetDefaults(shimmerOverride.ResultType);

                        self.shimmered = true;
                        self.stack = originalStack;

                        if (self.stack > 0)
                            self.shimmerTime = 1f;
                        else
                            self.shimmerTime = 0f;

                        self.shimmerWet = true;
                        self.wet = true;
                        self.velocity *= 0.1f;

                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Item.ShimmerEffect(self.Center);
                        }
                        else
                        {
                            NetMessage.SendData(146, -1, -1, null, 0, (int)self.Center.X, (int)self.Center.Y);
                            NetMessage.SendData(145, -1, -1, null, self.whoAmI, 1f);
                        }

                        AchievementsHelper.NotifyProgressionEvent(27);
                        if (self.stack == 0)
                        {
                            self.makeNPC = -1;
                            self.active = false;
                        }

                        return;
                    }
                }
            }

            orig(self);
        }
    }
}
