using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Linq;
using Terraria.Localization;

namespace Macrocosm.Common.Bases.Buffs
{
    // TODO: MORE HOOKS! --Feldy
    /// <summary> A <see cref="ModBuff"/> which exposes and runs (if active) some <see cref="ModPlayer"/> hooks, <see cref="GlobalNPC"/> hooks, and more! </summary>
    public abstract class ComplexBuff : ModBuff
    {
        /// <summary> Get all registered <see cref="ComplexBuff"/>s </summary>
        public static IEnumerable<ComplexBuff> Buffs => ModContent.GetContent<ComplexBuff>();

        /// <summary> Get all active <see cref="ComplexBuff"/>s on this <paramref name="npc"/> </summary>
        public static IEnumerable<ComplexBuff> GetActive(NPC npc) => Buffs.Where(buff => npc.HasBuff(buff.Type));

        /// <summary> Get all active <see cref="ComplexBuff"/>s on this <paramref name="player"/> </summary>
        public static IEnumerable<ComplexBuff> GetActive(Player player) => Buffs.Where(buff => player.HasBuff(buff.Type));


        // --- Player hooks ---

        /// <summary> Get a custom death message if the player dies while having this (de)buff </summary>
        public virtual LocalizedText GetCustomDeathMessage(Player player) => null;

        /// <inheritdoc cref="ModPlayer.CanUseItem(Item)"/>
        public virtual bool CanUseItem(Player player, Item item) => true;

        /// <inheritdoc cref="ModPlayer.UpdateLifeRegen()"/>
        public virtual void UpdateLifeRegen(Player player) { }

        /// <inheritdoc cref="ModPlayer.UpdateBadLifeRegen()"/>
        public virtual void UpdateBadLifeRegen(Player player) { }

        /// <inheritdoc cref="ModPlayer.DrawEffects(PlayerDrawSet, ref float, ref float, ref float, ref float, ref bool)()"/>
        public virtual void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) { }


        // --- NPC hooks ---

        /// <inheritdoc cref="GlobalNPC.UpdateLifeRegen(NPC, ref int)"></inheritdoc>
        public virtual void UpdateLifeRegen(NPC npc, ref int damage) { }

        /// <inheritdoc cref="GlobalNPC.ModifyHitByItem(NPC, Player, Item, ref NPC.HitModifiers)"></inheritdoc>
        public virtual void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) { }

        /// <inheritdoc cref="GlobalNPC.OnHitByItem(NPC, Player, Item, NPC.HitInfo, int)"></inheritdoc>
        public virtual void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) { }

        /// <inheritdoc cref="GlobalNPC.ModifyHitByProjectile(NPC, Projectile, ref NPC.HitModifiers)"></inheritdoc>
        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) { }

        /// <inheritdoc cref="GlobalNPC.OnHitByProjectile(NPC, Projectile, NPC.HitInfo, int)"></inheritdoc>
        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) { }

        /// <inheritdoc cref="GlobalNPC.ModifyHitByProjectile(NPC, Projectile, ref NPC.HitModifiers)"></inheritdoc>
        public virtual void HitEffect(NPC npc, NPC.HitInfo hit) { }

        /// <inheritdoc cref="GlobalNPC.OnKill(NPC)"></inheritdoc>
        public virtual void OnKill(NPC npc) { }

        /// <inheritdoc cref="GlobalNPC.ModifyHitNPC(NPC, NPC, ref NPC.HitModifiers)"></inheritdoc>
        public virtual void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers) { }

        /// <inheritdoc cref="GlobalNPC.ModifyHitPlayer(NPC, Player, ref Player.HurtModifiers)"></inheritdoc>
        public virtual void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers) { }

        /// <inheritdoc cref="GlobalNPC.DrawEffects(NPC, ref Color)"></inheritdoc>
        public virtual void DrawEffects(NPC npc, ref Color drawColor) { }
    }
}
