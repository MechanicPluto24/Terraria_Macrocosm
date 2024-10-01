using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Macrocosm.Content.Debuffs
{
    public class Melting : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;

            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.OnFire);
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.OnFire3);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 30;
            DustEffects(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 30;
            DustEffects(npc);
        }

        private void DustEffects(Entity entity)
        {
            int type;
            float scale = Main.rand.NextFloat(2f, 3f);
            if (Main.rand.NextBool(3))
            {
                type = DustID.SolarFlare;
                scale *= 0.5f;
            }
            else
            {
                type = DustID.Torch;
            }

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