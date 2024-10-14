using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Unlockables
{
    public class HelixPatternDesign : PatternDesign
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override List<(string moduleName, string patternName)> Patterns =>
        [
            ("CommandPod", "Helix"),
            ("ServiceModule", "Helix"),
            ("ReactorModule", "Helix"),
            ("EngineModule", "Helix"),
            ("BoosterLeft", "Helix"),
            ("BoosterRight", "Helix"),
        ];
    }
}
