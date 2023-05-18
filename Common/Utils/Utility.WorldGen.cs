using System;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static bool IsAprilFools() => DateTime.Now.Month == 4 && DateTime.Now.Day == 1;
    }
}