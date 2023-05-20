using Macrocosm.Common.Global.GlobalItems;
using Macrocosm.Common.Global.GlobalNPCs;
using Macrocosm.Common.Global.GlobalProjectiles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rocket;
using Terraria;

namespace Macrocosm.Common.Utils
{
    public static class Extension
    {
        public static MacrocosmProjectile Macrocosm(this Projectile projectile)
            => projectile.GetGlobalProjectile<MacrocosmProjectile>();

        public static MacrocosmPlayer Macrocosm(this Player player)
            => player.GetModPlayer<MacrocosmPlayer>();

		public static void AddScreenshake(this Player player, float value) 
            => player.Macrocosm().ScreenShakeIntensity += value;
        
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
