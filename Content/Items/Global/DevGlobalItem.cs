using Macrocosm.Content.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Global
{
    public interface IDevItem { }

    public class DevGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem is IDevItem;

        public override void SetDefaults(Item entity)
        {
            entity.rare = ModContent.RarityType<DevRarity>();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "DevItem", Language.GetText("Mods.Macrocosm.Common.DevItem").Value));
        }
    }
}
