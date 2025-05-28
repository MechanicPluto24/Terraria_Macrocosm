using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Linq;
using Terraria.Localization;

namespace Macrocosm.Common.Bases.Buffs
{
    // TODO: more hooks
    /// <summary>
    /// A ModBuff which exposes ModPlayer and GlobalNPC hooks
    /// </summary>
    public class ComplexBuff : ModBuff
    {
        public static IEnumerable<ComplexBuff> Buffs => ModContent.GetContent<ComplexBuff>();
        public static IEnumerable<ComplexBuff> GetActive(NPC npc) => Buffs.Where(buff => npc.HasBuff(buff.Type));
        public static IEnumerable<ComplexBuff> GetActive(Player player) => Buffs.Where(buff => player.HasBuff(buff.Type));

        public virtual LocalizedText GetCustomDeathMessage(Player player) => null;

        public virtual bool CanUseItem(Player player, Item item)
        {
            return true;
        }


        public virtual void UpdateLifeRegen(Player player)
        {
        }

        public virtual void UpdateBadLifeRegen(Player player)
        {
        }

        public virtual void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
        }

        // NPC hooks
        public virtual void AI(NPC npc)
        {
        }

        public virtual void UpdateLifeRegen(NPC npc, ref int damage)
        {
        }

        public virtual void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
        }

        public virtual void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
        }

        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
        }

        public virtual void HitEffect(NPC npc, NPC.HitInfo hit)
        {
        }

        public virtual void OnKill(NPC npc)
        {
        }

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
        }

        public virtual void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public virtual void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public virtual void DrawEffects(NPC npc, ref Color drawColor)
        {
        }

        // Player hooks
    }
}
