using Newtonsoft.Json.Linq;
using System;
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

        public static void PrettyPrintJSON(ref string text)
        {
            string[] array = text.Split(new string[1] { "\r\n" }, StringSplitOptions.None);
            foreach (string text2 in array)
            {
                if (text2.Contains(": {"))
                {
                    string text3 = text2[..text2.IndexOf('"')];
                    string text4 = text2 + "\r\n  ";
                    string newValue = text4.Replace(": {\r\n  ", ": \r\n" + text3 + "{\r\n  ");
                    text = text.Replace(text4, newValue);
                }
            }

            text = text.Replace("[\r\n        ", "[");
            text = text.Replace("[\r\n      ", "[");
            text = text.Replace("\"\r\n      ", "\"");
            text = text.Replace("\",\r\n        ", "\", ");
            text = text.Replace("\",\r\n      ", "\", ");
            text = text.Replace("\r\n    ]", "]");
        }

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