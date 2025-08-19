using Macrocosm.Common.Drawing;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items;

public class CommonTooltipGlobalItem : GlobalItem
{
    public override bool AppliesToEntity(Item item, bool lateInstantiation) => true;

    public override void SetDefaults(Item item)
    {
        if (ItemSets.DeveloperItem[item.type])
            item.rare = ModContent.RarityType<DevRarity>();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (ItemSets.DeveloperItem[item.type])
            tooltips.Add(new TooltipLine(Mod, "DeveloperItem", Language.GetText("Mods.Macrocosm.Tooltips.DeveloperItem").Value) { OverrideColor = CelestialDisco.CelestialColor });

        if (ItemSets.UnobtainableItem[item.type])
            tooltips.Add(new TooltipLine(Mod, "UnobtainableItem", Language.GetText("Mods.Macrocosm.Tooltips.UnobtainableItem").Value) { OverrideColor = Color.Red });
    }
}
