using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class ProjectileDistancePlayer : ModPlayer
    {
        public bool PointBlank { get; set; }
        public bool Zoning { get; set; }

        public override void ResetEffects()
        {
            PointBlank = false;
            Zoning = false;
        }
    }
}
