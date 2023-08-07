using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using Terraria;
using Macrocosm.Content.Projectiles.Global;
using Macrocosm.Content.Items.Global;
using System;
using Terraria.Utilities;

namespace Macrocosm.Common.Utils
{
    public static class Extension
    {
        public static MacrocosmProjectile Macrocosm(this Projectile projectile)
            => projectile.GetGlobalProjectile<MacrocosmProjectile>();

        public static MacrocosmPlayer Macrocosm(this Player player)
            => player.GetModPlayer<MacrocosmPlayer>();

		public static void AddScreenshake(this Player player, float value, string context) 
            => player.Macrocosm().AddScreenshake(value, context);
        
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

        public static int Next(this UnifiedRandom random, Range range)
        {
            return random.Next(range.Start.Value, range.End.Value);
        }

        public static int NextDirection(this UnifiedRandom random, Range range)
        {
            return random.Next(range) * (random.NextBool() ? 1 : -1);
        }
    }
}
