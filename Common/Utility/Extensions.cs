using Macrocosm.Common.Global.GlobalItems;
using Macrocosm.Common.Global.GlobalNPCs;
using Macrocosm.Common.Global.GlobalProjectiles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rocket;
using Terraria;

namespace Macrocosm.Common.Utility
{
    public static class Extension
    {
        public static MacrocosmGlobalProjectile Macrocosm(this Projectile projectile)
            => projectile.GetGlobalProjectile<MacrocosmGlobalProjectile>();

        public static MacrocosmPlayer Macrocosm(this Player player)
            => player.GetModPlayer<MacrocosmPlayer>();

        public static DashPlayer DashPlayer(this Player player)
            => player.GetModPlayer<DashPlayer>();

        public static StaminaPlayer StaminaPlayer(this Player player)
            => player.GetModPlayer<StaminaPlayer>();

        public static RocketPlayer RocketPlayer(this Player player)
            => player.GetModPlayer<RocketPlayer>();

        public static MacrocosmNPC Macrocosm(this NPC npc)
            => npc.GetGlobalNPC<MacrocosmNPC>();

        public static GlowmaskGlobalItem Glowmask(this Item item)
            => item.GetGlobalItem<GlowmaskGlobalItem>();
    }
}
