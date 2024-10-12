using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Items.Accessories
{
    public class ThaumaturgicWard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ManaWardPlayer>().ManaWard = true;
            player.GetDamage<GenericDamageClass>() -= 100;
            player.GetDamage<MagicDamageClass>() += 100;
        }
    }

    public class ManaWardPlayer : ModPlayer
    {
        public bool ManaWard;
        public override void ResetEffects()
        {
            ManaWard = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (ManaWard)
            {
                int manaDamage = 0;
                if ((float)Player.statLife / Player.statLifeMax2 < 0.25f)
                {
                    manaDamage = info.Damage;
                    if (manaDamage > Player.statMana) manaDamage = Player.statMana;
                    Player.statMana -= manaDamage;
                    Player.statLife -= info.Damage - manaDamage;
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (ManaWard && (float)Player.statLife / Player.statLifeMax2 < 0.25f) modifiers.FinalDamage *= 0.5f;
        }
    }
}
