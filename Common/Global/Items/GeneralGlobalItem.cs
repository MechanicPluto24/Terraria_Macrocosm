using Macrocosm.Common.Players;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Items.Accessories;
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
    public class GeneralGlobalItem : GlobalItem
    {
        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
        {
            //Main.NewText("test");
            //Probably a way to write this more efficiently but this can be done later.
            if (item.type == ModContent.ItemType<ThaumaturgicWard>() && player.magicCuffs) return false;
            else if (item.type == ItemID.MagicCuffs && player.GetModPlayer<ManaWardPlayer>().ManaWard) return false;
            else return base.CanEquipAccessory(item, player, slot, modded);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.MagicCuffs) tooltips.Add(new TooltipLine(Macrocosm.Instance, "Cuff/Ward Exclusivity", "Cannot be equipped with Thaumaturgic Ward"));
        }
    }
}
