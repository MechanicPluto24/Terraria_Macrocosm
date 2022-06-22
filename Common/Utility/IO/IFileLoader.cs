using System;

namespace Macrocosm.Common.Utility.IO {
    public interface IFileLoader<T> {
        T Parse(string path);
    }
}
