
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Unlockables
{
    public abstract class PatternDesign : ModItem
    {
        public abstract List<(string moduleName, string patternName)> Patterns { get; }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
        }
    }
}
