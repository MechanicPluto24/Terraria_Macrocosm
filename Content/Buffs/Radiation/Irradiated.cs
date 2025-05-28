using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Radiation
{
    public class Irradiated : ComplexBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            npc.lifeRegen -= 30;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense = (int)(npc.defense * 0.9f);
            npc.damage = (int)(npc.damage * 0.9f);
            DustEffects(npc);
        }

        private void DustEffects(Entity entity)
        {
            int type;
            float scale = Main.rand.NextFloat(2f, 3f);

            type = ModContent.DustType<IrradiatedDust>();
            Dust dust = Dust.NewDustDirect(new Vector2(entity.position.X - 2f, entity.position.Y - 12f), entity.width + 4, entity.height - 12, type, entity.velocity.X * 0.4f, entity.velocity.Y * 0.4f, 100, default, scale);
            dust.velocity.Y += 2.5f;
            if (dust.velocity.Y < 0)
                dust.velocity.Y *= -1;

            dust.noGravity = true;

            if (Main.rand.NextBool(2))
                dust.scale *= 0.1f;
        }
    }
}