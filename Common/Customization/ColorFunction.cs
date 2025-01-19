using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
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

        private readonly Func<Dictionary<Color, Color>, object[], Color> function;

        private ColorFunction(string name, Func<Dictionary<Color, Color>, object[], Color> function, object[] parameters)
        {
            Name = name;
            this.function = function ?? throw new ArgumentNullException(nameof(function));
            Parameters = parameters ?? Array.Empty<object>();
        }

        public Color Invoke(Dictionary<Color, Color> availableColors)
        {
            try
            {
                return function(availableColors, Parameters);
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
            Macrocosm.Instance.Logger.Info($"Creating ColorFunction: {name} with parameters: {string.Join(", ", parameters)}");
            switch (name.ToLower())
            {
                case "lerp":
                    return CreateLerpFunction(parameters);

                default:
                    LogError($"Unknown ColorFunction name: {name}");
                    return new ColorFunction(name, (_, _) => Color.Transparent, Array.Empty<object>());
            }
        }

        private static ColorFunction CreateLerpFunction(object[] parameters)
        {
            if (parameters.Length != 3)
            {
                LogError("Lerp function requires exactly 3 parameters: [key1, key2, amount].");
                return new ColorFunction("lerp", (_, _) => Color.Transparent, parameters);
            }

            var parsedParams = ParseParameters(parameters.Select(p => p.ToString()).ToArray());
            if (parsedParams.Length != 3 || parsedParams[0] is not Color key1 || parsedParams[1] is not Color key2 || parsedParams[2] is not float amount || amount < 0f || amount > 1f)
            {
                LogError("Lerp function requires valid parameters: [Color key1, Color key2, float amount (0-1)].");
                return new ColorFunction("lerp", (_, _) => Color.Transparent, parameters);
            }

            return new ColorFunction("lerp", (colors, args) =>
            {
                if (!colors.TryGetValue((Color)args[0], out var color1))
                {
                    LogError($"Lerp: Color key not found: {args[0]}");
                    return Color.Transparent;
                }

                if (!colors.TryGetValue((Color)args[1], out var color2))
                {
                    LogError($"Lerp: Color key not found: {args[1]}");
                    return Color.Transparent;
                }

                return Color.Lerp(color1, color2, (float)args[2]);
            }, parameters);
        }

        private static object[] ParseParameters(string[] parameters)
        {
            return parameters.Select<string, object>(param =>
            {
                if (int.TryParse(param, out int intValue))
                    return intValue;

                if (double.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                    return doubleValue;

                if (float.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                    return floatValue;

                if (Utility.TryGetColorFromHex(param, out Color colorValue))
                    return colorValue;

                return param;
            }).ToArray();
        }

        private static void LogError(string message)
        {
            Utility.Chat(message, Color.Orange);
            Macrocosm.Instance.Logger.Error(message);
        }
    }
}
