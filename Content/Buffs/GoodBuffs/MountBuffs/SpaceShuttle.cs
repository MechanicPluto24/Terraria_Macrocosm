using Macrocosm.Content.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs.MountBuffs
{
    public class SpaceShuttle : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<SpaceShuttleMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.armorEffectDrawShadow = true;
            var checkDamagePlayer = player.getRect();
            checkDamagePlayer.Offset(0, player.height - 1);
            checkDamagePlayer.Inflate(12, 6);

        }
    }
}