using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Players;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Environment;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

public class DeathMessageHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_Player.KillMe += On_Player_KillMe;
    }

    public void Unload()
    {
        On_Player.KillMe -= On_Player_KillMe;
    }

    private void On_Player_KillMe(On_Player.orig_KillMe orig, Player player, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
    {
        // 8 is the Burning "other" death message type.
        // Bad life regen (a.k.a. damage over time) deaths default to this
        // Here we are overriding it to our custom reasons.
        // Only overrided on the local client, will be synced and message will be broadcast by the server 
        if (damageSource.SourceOtherIndex == 8 && player.whoAmI == Main.myPlayer)
        {
            if (player.GetModPlayer<IrradiationPlayer>().IrradiationLevel > 0f)
            {
                damageSource.CustomReason = IrradiationPlayer.DeathMessages.GetRandom().ToNetworkText(player.name);
            }
            else 
            {
                foreach (var buff in ComplexBuff.Buffs)
                {
                    if (player.HasBuff(buff.Type))
                    {
                        var customMessage = buff.GetCustomDeathMessage(player);
                        if (customMessage != null)
                        {
                            damageSource.CustomReason = customMessage.ToNetworkText(player.name);
                            break;
                        }
                    }
                }

            }
        }

        orig(player, damageSource, dmg, hitDirection, pvp);
    }
}
