using Macrocosm.Common.Bases.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class PotionDelayHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            On_Player.ApplyPotionDelay += On_Player_ApplyPotionDelay;
        }
        public void Unload()
        {
            On_Player.ApplyPotionDelay -= On_Player_ApplyPotionDelay;
        }

        private void On_Player_ApplyPotionDelay(On_Player.orig_ApplyPotionDelay orig, Player self, Item sItem)
        {
            if(sItem.ModItem is IPotionDelayConsumable consumable)
            {
                self.potionDelay = consumable.PotionDelay;

                if (self.pStone)
                    self.potionDelay = (int)((float)self.potionDelay * Player.PhilosopherStoneDurationMultiplier);

                self.AddBuff(BuffID.PotionSickness, self.potionDelay);
            }
            else
            {
                orig(self, sItem);
            }
        }
    }
}
