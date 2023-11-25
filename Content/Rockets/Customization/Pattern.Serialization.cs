using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public partial class Pattern : TagSerializable
    {
        public string ToJSON(bool includeColorFunctions = false, bool includeUnlocked = false, bool includeNonModifiableColors = false) => ToJObject(includeColorFunctions, includeUnlocked, includeNonModifiableColors).ToString(Formatting.Indented);

        public JObject ToJObject(bool includeColorFunctions = true, bool includeUnlocked = false, bool includeNonModifiableColors = false)
        {
            JObject jsonObject = new()
            {
                ["patternName"] = Name,
                ["moduleName"] = ModuleName
            };

            if (includeUnlocked)
            {
                jsonObject["unlocked"] = Unlocked;
                jsonObject["unlockedByDefault"] = UnlockedByDefault;
            }

            JArray colorDataArray = new();
            foreach (var colorData in ColorData)
            {
                JObject colorDataObject = new();

                if (colorData.HasColorFunction)
                {
                    if (includeColorFunctions)
                    {
                        colorDataObject["colorFunction"] = colorData.ColorFunction.Name;
                        colorDataObject["params"] = new JArray(colorData.ColorFunction.Parameters);
                    }
                    else
                    {
                        colorDataObject = new JObject();
                    }
                }
                else
                {
                    if (includeNonModifiableColors || colorData.IsUserModifiable)
                    {
                        colorDataObject["color"] = colorData.Color.GetHexText();

                        if (!colorData.IsUserModifiable)
                            colorDataObject["userModifiable"] = false;
                    }
                    else
                    {
                        colorDataObject = new JObject();
                    }
                }

                colorDataArray.Add(colorDataObject);
            }

            // Remove trailing empty JObjects
            for (int i = colorDataArray.Count - 1; i >= 0; i--)
            {
                if (colorDataArray[i].Type == JTokenType.Object && !colorDataArray[i].HasValues)
                    colorDataArray.RemoveAt(i);
                else
                    break;
            }


            jsonObject["colorData"] = colorDataArray;

            return jsonObject;
        }

        public static Pattern FromJSON(string json) => FromJObject(JObject.Parse(json));

        public static Pattern FromJObject(JObject patternObject)
        {
            string moduleName = patternObject.Value<string>("moduleName");

            if (!Rocket.DefaultModuleNames.Contains(moduleName))
                throw new SerializationException("Error: Invalid module name.");

            string patternName = patternObject.Value<string>("patternName");

            if (CustomizationStorage.Initialized && !CustomizationStorage.TryGetPattern(moduleName, patternName, out _))
                throw new SerializationException("Error: Invalid pattern name.");

            bool unlockedByDefault = patternObject.Value<bool>("unlockedByDefault");

            JArray colorDataArray = patternObject.Value<JArray>("colorData");
            List<PatternColorData> colorDatas = new();

            foreach (JObject colorDataObject in colorDataArray.Cast<JObject>())
            {
                string colorHex = colorDataObject.Value<string>("color");
                string colorFunction = colorDataObject.Value<string>("colorFunction");

                if (!string.IsNullOrEmpty(colorFunction))
                {
                    JArray parameters = colorDataObject.Value<JArray>("params");
                    colorDatas.Add(new(ColorFunction.CreateFunctionByName(colorFunction, parameters?.ToObjectRecursive<object>())));
                }
                else if (!string.IsNullOrEmpty(colorHex))
                {
                    bool userModifiable = colorDataObject.Value<bool?>("userModifiable") ?? true;

                    if (Utility.TryGetColorFromHex(colorHex, out Color color))
                        colorDatas.Add(new(color, userModifiable));
                    else
                        throw new SerializationException($"Error: Invalid color code: {colorHex}.");
                }
                else
                {
                    colorDatas.Add(new());
                }
            }

            return new Pattern(moduleName, patternName, unlockedByDefault, colorDatas.ToArray());
        }

        public Pattern Clone() => DeserializeData(SerializeData());

        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

        // TODO: a way to also save and load custom functions just like the JSON does
        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                ["Name"] = Name,
                ["ModuleName"] = ModuleName
            };

            // Save the user-changed colors
            for (int i = 0; i < MaxColorCount; i++)
                if (ColorData[i].IsUserModifiable)
                    tag[$"Color{i}"] = ColorData[i].Color;

            return tag;
        }

        public static Pattern DeserializeData(TagCompound tag)
        {
            string name = tag.GetString("Name");
            string moduleName = tag.GetString("ModuleName");

            Pattern originalPatternData = CustomizationStorage.GetPatternReference(moduleName, name);
            Pattern pattern = new(moduleName, name, originalPatternData.UnlockedByDefault, originalPatternData.ColorData);

            // Load the user-changed colors
            for (int i = 0; i < MaxColorCount; i++)
                if (tag.ContainsKey($"Color{i}") && pattern.ColorData[i].IsUserModifiable)
                    pattern.ColorData[i] = pattern.ColorData[i].WithUserColor(tag.Get<Color>($"Color{i}"));

            return pattern;
        }

        /*
		// This is terribly inneficient 
		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				{ "Pattern", ToJSON(includeColorFunctions: true, includeUnlocked: false, includeNonModifiableColors: true) }
			};
			return tag;
		}

		public static Pattern DeserializeData(TagCompound tag)
		{
			if (tag.ContainsKey("Pattern"))
			{
				string json = tag.GetString("Pattern");
				Pattern pattern = FromJSON(json);
				return pattern;
			}

			return new Pattern("", "", false);
		}
		*/

    }
}
