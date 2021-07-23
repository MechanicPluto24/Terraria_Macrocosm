using System;
using System.Collections.Generic;

namespace Macrocosm.Common.Utility
{
    public class ListRandom
    {
        public static T Pick<T>(IList<T> list) {
            return list[new Random().Next(list.Count)];
        } 
    }
}
