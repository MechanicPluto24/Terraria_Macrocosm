using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Buffs
{
    public class ComplexBuff : ModBuff
    {
        public virtual void ModifyAI(NPC npc) { }
        public virtual void DrawEffects(NPC npc, ref Color drawColor) { }
        public virtual void UpdateLifeRegen(NPC npc, ref int damage) { }
        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) { }
        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) { }
        public virtual void UpdatePlayer(Player player) { }
    }
}
