using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Content.Buffs.Weapons;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Macrocosm.Common.Global.NPCs
{
    public class ComplexBuffGlobalNPC : GlobalNPC
    {
        public override void AI(NPC npc)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.AI(npc);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.UpdateLifeRegen(npc, ref damage);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.ModifyHitByItem(npc, player, item, ref modifiers);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.OnHitByItem(npc, player, item, hit, damageDone);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.ModifyHitByProjectile(npc, projectile, ref modifiers);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.OnHitByProjectile(npc, projectile, hit, damageDone);
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.HitEffect(npc, hit);
        }

        public override void OnKill(NPC npc)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.OnKill(npc);
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.ModifyHitNPC(npc, target, ref modifiers);
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.ModifyHitPlayer(npc, target, ref modifiers);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            foreach (var buff in ComplexBuff.GetActive(npc))
                buff.DrawEffects(npc, ref drawColor);
        }
    }
}
