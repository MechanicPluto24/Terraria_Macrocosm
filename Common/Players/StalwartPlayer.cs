using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class StalwartPlayer : ModPlayer
    {
        public bool StalwartShield { get; set; }

        private int defenceBonus;
        private int decreaseTick;

        public override void ResetEffects()
        {
            StalwartShield = false;
        }

        public override void PostUpdateEquips()
        {
            if (StalwartShield)
            {
                if (decreaseTick < 180 && defenceBonus > 0)
                {
                    decreaseTick++;
                }

                if (decreaseTick >= 180)
                {
                    defenceBonus -= 3;
                    decreaseTick = 30;
                }

                Player.statDefense += defenceBonus;
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (StalwartShield)
            {
                if (defenceBonus < 12)
                    defenceBonus += 3;

                decreaseTick = 0;
            }
        }
    }
}
