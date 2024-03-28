using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Loot
{
    public interface IBlacklistable
    {
        public int ItemID { get; }
        public bool Blacklisted { get; set; }
    }
}
