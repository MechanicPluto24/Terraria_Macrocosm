using Macrocosm.Content.Buffs.Potions;
using Macrocosm.Content.Items.Consumables.Potions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Players
{
    public class MedkitPlayer : ModPlayer
    {
        public bool MedkitActive => Player.HasBuff<MedkitLow>() || Player.HasBuff<MedkitMedium>() || Player.HasBuff<MedkitHigh>();

        // Used for identifying medkit tier
        public int MedkitItemType = ModContent.ItemType<Medkit>();
        private Medkit Medkit => (ContentSamples.ItemsByType[MedkitItemType].ModItem as Medkit);
        private int medkitTimer;
        private int medkitHitCooldown;

        public override void PostUpdateBuffs()
        {
            if (MedkitActive)
            {
                int healPeriod = Medkit.HealPeriod;
                if (medkitTimer++ >= healPeriod)
                {
                    medkitTimer = 0;
                    int healAmount = Medkit.HealthPerPeriod;

                    if (Player.HasBuff<MedkitLow>())
                        Player.Heal((int)(healAmount * 0.33f));
                    else if (Player.HasBuff<MedkitMedium>())
                        Player.Heal((int)(healAmount * 0.66f));
                    else if (Player.HasBuff<MedkitHigh>())
                        Player.Heal(healAmount);
                }

                if (medkitHitCooldown > 0)
                    medkitHitCooldown--;
            }
            else
            {
                medkitTimer = 0;
                medkitHitCooldown = 0;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (medkitHitCooldown > 0)
                return;

            if (Player.HasBuff<MedkitMedium>())
            {
                int index = Player.FindBuffIndex(ModContent.BuffType<MedkitMedium>());
                int time = Player.buffTime[index];
                Player.DelBuff(index);
                Player.AddBuff(ModContent.BuffType<MedkitLow>(), time);

                medkitHitCooldown = Medkit.HitCooldown;
            }

            if (Player.HasBuff<MedkitHigh>())
            {
                int index = Player.FindBuffIndex(ModContent.BuffType<MedkitHigh>());
                int time = Player.buffTime[index];
                Player.DelBuff(index);
                Player.AddBuff(ModContent.BuffType<MedkitMedium>(), time);

                medkitHitCooldown = Medkit.HitCooldown;
            }
        }
    }
}
