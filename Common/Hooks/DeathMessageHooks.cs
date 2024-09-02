using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Environment;
using Macrocosm.Content.Debuffs.Radiation;
using Macrocosm.Content.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
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

        private void On_Player_KillMe(On_Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
        {
            // 8 is the Burning "other" death message type.
            // Bad life regen (a.k.a. damage over time) deaths default to this
            // Here we are overriding it to our custom reasons.
            if(damageSource.SourceOtherIndex == 8)
            {
                if (self.GetModPlayer<IrradiationPlayer>().IrradiationLevel > 0f) 
                    damageSource.SourceCustomReason = IrradiationPlayer.DeathMessages.GetRandom().Format(self.name);
                else if (self.HasBuff(ModContent.BuffType<Depressurized>()))
                    damageSource.SourceCustomReason = Depressurized.DeathMessages.GetRandom().Format(self.name);
            }

            orig(self, damageSource, dmg, hitDirection, pvp);
        }
    }
}
