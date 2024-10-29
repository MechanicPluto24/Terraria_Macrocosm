using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class WeaponBalanceGlobalItem : GlobalItem
    {
      
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                damage *= ItemSets.DamageAdjustment[item.type];
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                float adjustment = ItemSets.DamageAdjustment[item.type];
                if (adjustment < 1f)
                    tooltips.Add(new TooltipLine(Macrocosm.Instance, "WeaponNerf", Language.GetTextValue("Mods.Macrocosm.Common.Tooltips.WeaponNerf")) { OverrideColor = Color.GreenYellow });
                else if (adjustment > 1f)
                    tooltips.Add(new TooltipLine(Macrocosm.Instance, "WeaponBuff", Language.GetTextValue("Mods.Macrocosm.Common.Tooltips.WeaponBuff")) { OverrideColor = Color.GreenYellow });
            }            
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
        }
    }
}
