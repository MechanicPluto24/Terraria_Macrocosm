using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
