using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class WingPlayer : ModPlayer
    {
        public override void Load()
        {
            On_Player.GetWingStats += On_Player_GetWingStats;
            On_Player.WingMovement += On_Player_WingMovement;
        }

        public override void Unload()
        {
            On_Player.GetWingStats -= On_Player_GetWingStats;
        }

        public float SoaringInsigniaWingTimeDecrease => 0.25f;

        private WingStats On_Player_GetWingStats(On_Player.orig_GetWingStats orig, Player player, int wingID)
        {
            WingStats stats = orig(player, wingID);
            int wingItemType = Utility.GetItemTypeFromWingID(wingID);

            if (SubworldSystem.AnyActive<Macrocosm>() && wingItemType > 0)
            {
                if (SubworldSystem.IsActive<Moon>())
                    stats.FlyTime = (int)(stats.FlyTime * ItemSets.WingTimeMultiplier_Moon[wingItemType]);

                if (ItemSets.WingTimeDependsOnAtmosphericDensity[wingItemType])
                    stats.FlyTime = (int)(stats.FlyTime * MacrocosmSubworld.CurrentAtmosphericDensity);

                player.wingTimeMax = stats.FlyTime;
            }

            return stats;
        }

        private void On_Player_WingMovement(On_Player.orig_WingMovement orig, Player player)
        {
            // Disable Soaring Insignia infinite flight
            // Still gives a bit of extra flight time 
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                bool empressBrooch = player.empressBrooch;
                float wingTime = player.wingTime;

                player.empressBrooch = false;

                orig(player);

                player.empressBrooch = empressBrooch;

                if (player.empressBrooch && player.wingTime != 0f)
                    player.wingTime = wingTime - SoaringInsigniaWingTimeDecrease;
            }
            else
            {
                orig(player);
            }
        }
    }
}
