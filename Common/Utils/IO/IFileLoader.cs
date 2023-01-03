namespace Macrocosm.Common.Utils.IO
{
	public interface IFileLoader<T>
	{
		T Parse(string path);
	}
}
