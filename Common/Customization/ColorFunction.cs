using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Macrocosm.Common.Customization
{
    public class ColorFunction
    {
        public string Name { get; }
        public object[] Parameters { get; }

        private readonly Func<Dictionary<Color, Color>, Color> function;

        private ColorFunction(string name, Func<Dictionary<Color, Color>, Color> function, object[] parameters)
        {
            Name = name;
            Parameters = parameters ?? Array.Empty<object>();
            this.function = function ?? throw new ArgumentNullException(nameof(function));
        }

        public Color Invoke(Dictionary<Color, Color> availableColors)
        {
            try
            {
                return function(availableColors);
            }
            catch (Exception ex)
            {
                LogError($"Error invoking ColorFunction '{Name}': {ex.Message}");
                return default;
            }
        }

        public static ColorFunction CreateByName(string name) => CreateByName(name, Array.Empty<object>());

        public static ColorFunction CreateByName(string name, object[] parameters)
        {
            return name.ToLower() switch
            {
                "lerp" => CreateLerpFunction(parameters),
                _ => null,
            };
        }

        private static ColorFunction CreateLerpFunction(object[] inputParameters)
        {
            if (inputParameters.Length != 3)
            {
                LogError("Lerp function requires exactly 3 parameters: [key1, key2, amount].");
                return new ColorFunction("default", _ => Color.Transparent, inputParameters);
            }

            var parsedParameters = ParseParameters(inputParameters.Select(p => p.ToString()).ToArray());
            if (parsedParameters.Length != 3 || parsedParameters[0] is not Color || parsedParameters[1] is not Color || parsedParameters[2] is not float)
            {
                LogError("Lerp function requires valid parameters: [Color key1, Color key2, float amount].");
                return new ColorFunction("default", _ => Color.Transparent, inputParameters);
            }

            return new ColorFunction("lerp", colors =>
            {
                if (!colors.TryGetValue((Color)parsedParameters[0], out var color1))
                    return Color.Transparent;

                if (!colors.TryGetValue((Color)parsedParameters[1], out var color2))
                    return Color.Transparent;

                return Color.Lerp(color1, color2, (float)parsedParameters[2]);
            }, parsedParameters);
        }

        public static object[] ParseParameters(string[] parameters)
        {
            return parameters.Select<string, object>(param =>
            {
                if (int.TryParse(param, out int intValue))
                    return intValue;

                if (double.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                    return (float)doubleValue;

                if (float.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                    return floatValue;

                if (Utility.TryGetColorFromHex(param, out Color colorValue))
                    return colorValue;

                return param;
            }).ToArray();
        }

        public static IEnumerable<object> UnparseParameters(object[] parameters)
        {
            foreach (var param in parameters)
            {
                switch (param)
                {
                    case Color color:
                        yield return color.GetHex();
                        break;
                    case float or int or string:
                        yield return param;
                        break;
                    default:
                        yield return param?.ToString();
                        break;
                }
            }
        }

        private static void LogError(string message)
        {
            Utility.Chat(message, Color.Orange);
            Macrocosm.Instance.Logger.Error(message);
        }
    }
}
