using System.Collections.Generic;

namespace Macrocosm.Content.Items.Consumables.Unlockables
{
    public class HelixPatternDesign : PatternDesign
    {
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
