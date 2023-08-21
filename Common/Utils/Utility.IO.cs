using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
        public static string GetTextFromFile(string path)
        {
            var bytes = Macrocosm.Instance.GetFileBytes(path);
			return System.Text.Encoding.UTF8.GetString(bytes);
        }

		public static JArray ParseJSONFromFile(string path)
			=> JArray.Parse(GetTextFromFile(path));

		public static T[] ToObjectRecursive<T>(this JArray jArray)
		{
			var result = new List<T>();

			foreach (var item in jArray)
			{
				if (item is JArray nestedArray)
 					result.AddRange(ToObjectRecursive<T>(nestedArray));
 				else
 					result.Add(item.ToObject<T>());
 			}

			return result.ToArray();
		}
	}
}