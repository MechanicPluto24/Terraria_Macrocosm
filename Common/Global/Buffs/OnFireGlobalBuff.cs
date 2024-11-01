using Macrocosm.Content.Debuffs;
using Terraria;
using SubworldLibrary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Buffs
{
    public class OnFireGlobalBuff : GlobalBuff
    {
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type is BuffID.OnFire or BuffID.OnFire3&&SubworldSystem.AnyActive<Macrocosm>())
            {
                int time = npc.buffTime[buffIndex];

                npc.DelBuff(buffIndex);
                buffIndex--;

                npc.onFire = false;
                npc.onFire3 = false;

                npc.AddBuff(ModContent.BuffType<Melting>(), time);
            }
        }

        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type is BuffID.OnFire or BuffID.OnFire3&&SubworldSystem.AnyActive<Macrocosm>())
            {
                int time = player.buffTime[buffIndex];

                player.DelBuff(buffIndex);
                buffIndex--;

                player.onFire = false;
                player.onFire3 = false;

                player.AddBuff(ModContent.BuffType<Melting>(), time);
            }
        }

    }
}
