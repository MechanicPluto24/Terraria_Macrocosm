namespace Macrocosm.Common.Utility.IO
{
	public class TextFileLoader : IFileLoader<string[]>
	{
		public string[] Parse(string path)
		{
			var bytes = Macrocosm.Instance.GetFileBytes($"{path}.txt");
			return System.Text.Encoding.UTF8.GetString(bytes).Split('\n');
		}
	}
}
