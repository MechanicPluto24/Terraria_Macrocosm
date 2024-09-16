using Macrocosm.Common.Sets;
using Macrocosm.Content.Rarities;
using System.Collections.Generic;
using System.Drawing;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class DevGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemSets.DeveloperItem[item.type];

        public override void SetDefaults(Item item)
        {
            item.rare = ModContent.RarityType<DevRarity>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "DevItem", Language.GetText("Mods.Macrocosm.Common.DevItem").Value));
        }
    }
}
