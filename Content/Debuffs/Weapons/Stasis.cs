using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Weapons
{
    internal class Stasis : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            DustEffects(player);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.dontTakeDamage)
                DustEffects(npc);
        }

        private void DustEffects(Entity entity)
        {
            if(Main.rand.NextBool())
            {
                Dust.NewDustDirect(entity.position, entity.width, entity.height, ModContent.DustType<Dusts.StasisDust>(), Scale: Main.rand.NextFloat(0.8f, 1.2f));
            }
        }
    }
}
