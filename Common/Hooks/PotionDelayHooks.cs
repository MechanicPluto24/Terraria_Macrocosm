using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

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
        int delay = ItemSets.PotionDelay[sItem.type];
        if (delay > 0)
        {
            self.potionDelay = delay;

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
