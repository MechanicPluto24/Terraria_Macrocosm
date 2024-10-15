using Macrocosm.Content.Projectiles.Friendly.Summon;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Minions
{
    public class GreatstaffOfHorusMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GreatstaffOfHorusMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
