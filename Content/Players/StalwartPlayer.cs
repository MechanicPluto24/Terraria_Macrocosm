using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
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
                if (decreaseTick < 60 && defenceBonus > 0)
                {
                    decreaseTick++;
                }

                if (decreaseTick >= 60)
                {
                    defenceBonus -= 5;
                    decreaseTick = 30;
                }

                Player.statDefense += defenceBonus;
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (StalwartShield)
            {
                if (defenceBonus < 30) 
                    defenceBonus += 10;

                decreaseTick = 0;
            }
        }
    }
}
