using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class StalwartTowerShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 52;
            Item.accessory = true;
            Item.defense = 10;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<StalwartPlayer>().StalwartShield = true;
        }
    }

    public class StalwartPlayer : ModPlayer
    {
        public bool StalwartShield;
        public int defenceBonus;
        public int decreaseTick;

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
                if (defenceBonus < 30) defenceBonus += 10;
                decreaseTick = 0;
            }
        }
    }
}
