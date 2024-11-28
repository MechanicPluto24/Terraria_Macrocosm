using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class UnusableGlobalItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && ItemSets.UnusableItems[item.type])
                return false;

            return true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                if(ItemSets.UnusableItems[item.type])
                    tooltips.Add(new TooltipLine(Macrocosm.Instance, "Unusable", Language.GetText("Mods.Macrocosm.Common.Tooltips.Unusable").Format(MacrocosmSubworld.Current.DisplayName.Value)) 
                    { 
                        OverrideColor = Color.Orange 
                    });
            }
        }
    }
}