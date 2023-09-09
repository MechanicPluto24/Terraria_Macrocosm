namespace Macrocosm.Common.IO
{
    public interface IFileLoader<T>
    {
        T Parse(string path);
    }
}
