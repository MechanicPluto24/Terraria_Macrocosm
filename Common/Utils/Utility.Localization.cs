using System.Text.RegularExpressions;
using Terraria.Localization;

namespace Macrocosm.Common.Utils;

public static partial class Utility
{
    public static string GetLanguageValueOrEmpty(string key)
    {
        string value = Language.GetTextValue(key);
        return (value == key) ? "" : value;
    }

    public static LocalizedText GetLocalizedTextOrEmpty(string key)
    {
        LocalizedText text = Language.GetText(key);
        return (text.Value == key) ? LocalizedText.Empty : text;
    }

    public static LocalizedText[] FindAllLocalizationThatStartsWith(string prefix) => Language.FindAll(new LanguageSearchFilter((key, _) => key.StartsWith(prefix)));

    public static string TrimTrailingZeroesAndDot(string decimalNumberText) => string.Format("{0:n}", decimalNumberText).TrimEnd('0').TrimEnd('.');

    public static string PrettyPrintName(string name) => Regex.Replace(name, "([A-Z])", " $1").Trim();
}
