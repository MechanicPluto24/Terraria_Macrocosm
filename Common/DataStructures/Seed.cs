using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Common.DataStructures
{
    internal class Seed
    {
        public int Value { get; }
        public static Seed Random => new(WorldGen.genRand.Next());
        public static Seed FromInt(int value) => new(value);
        private Seed(int value)
        {
            Value = value;
        }
    }
}
