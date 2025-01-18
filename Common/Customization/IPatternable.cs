using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macrocosm.Content.Rockets.Customization;

namespace Macrocosm.Common.Customization
{
    public interface IPatternable
    {
        public string PatternContext { get; }
        public Pattern Pattern { get; set; }
        public IEnumerable<Pattern> AvailablePatterns { get; }
    }

}
