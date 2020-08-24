using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Buffs.Buffs.MountBuffs
{
    public class SpaceShuttle : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Space Shuttle");
            Description.SetDefault("In memory of STS-51-L and STS-107.");
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType("SpaceShuttle"), player);
            player.buffTime[buffIndex] = 10;
            player.armorEffectDrawShadow = true;
            var checkDamagePlayer = player.getRect();
            checkDamagePlayer.Offset(0, player.height - 1);
            checkDamagePlayer.Inflate(12, 6);

        }
    }
}