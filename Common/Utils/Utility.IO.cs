namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
        public static string GetTextFromFile(string path)
        {
            var bytes = Macrocosm.Instance.GetFileBytes(path);
			return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}