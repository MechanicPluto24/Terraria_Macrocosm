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
	public readonly partial struct Pattern : TagSerializable
	{ 
		public string ToJSON(bool includeColorFunctions = false, bool includeNonModifiableColors = false) => ToJObject(includeColorFunctions, includeNonModifiableColors).ToString(Formatting.Indented);

		public JObject ToJObject(bool includeColorFunctions = true, bool includeNonModifiableColors = false)
		{
			JObject jObject = new()
			{
				["patternName"] = Name,
				["moduleName"] = ModuleName
			};

			JArray colorDataArray = new();
			foreach (var colorData in ColorData)
			{
				JObject colorDataObject = new();

				if (colorData.HasColorFunction)
				{
					if (includeColorFunctions)
					{
						colorDataObject["colorFunction"] = colorData.ColorFunction.Name;
						colorDataObject["parameters"] = new JArray(colorData.ColorFunction.Parameters);
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
						colorDataObject["color"] = colorData.Color.GetHex();

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
 
			jObject["colorData"] = colorDataArray;

			return jObject; 
		}

        public static Pattern FromJSON(string json) => FromJObject(JObject.Parse(json));

		public static Pattern FromJObject(JObject jObject)
		{
			string moduleName = jObject.Value<string>("moduleName");

            if (!Rocket.DefaultModuleNames.Contains(moduleName))
                throw new SerializationException("Error: Invalid module name.");

			string patternName = jObject.Value<string>("patternName");

            if (CustomizationStorage.Initialized && !CustomizationStorage.TryGetPattern(moduleName, patternName, out _))
                throw new SerializationException("Error: Invalid pattern name.");

			JArray colorDataArray = jObject.Value<JArray>("colorData");
			List<PatternColorData> colorDatas = new();

            foreach (JObject colorDataObject in colorDataArray.Cast<JObject>())
            {
                string colorHex = colorDataObject.Value<string>("color");
                string colorFunction = colorDataObject.Value<string>("colorFunction");

				if (!string.IsNullOrEmpty(colorFunction))
				{
					JArray parameters = colorDataObject.Value<JArray>("parameters");
                    colorDatas.Add(new PatternColorData(ColorFunction.CreateByName(colorFunction, parameters?.ToObjectRecursive<object>())));
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

			return new Pattern(moduleName, patternName, colorDatas.ToArray());
		}


        public static readonly Func<TagCompound, Pattern> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				[nameof(Name)] = Name,
				[nameof(ModuleName)] = ModuleName,
				[nameof(ColorData)] = ColorData.ToList()
			};

			return tag;
		}

		public static Pattern DeserializeData(TagCompound tag)
		{
			string name = "";
			string moduleName = "";
			Pattern pattern;

			try
			{
				if (tag.ContainsKey(nameof(Name)))
					name = tag.GetString(nameof(Name));

				if (tag.ContainsKey(nameof(ModuleName)))
					moduleName = tag.GetString(nameof(ModuleName));

				pattern = CustomizationStorage.GetPattern(moduleName, name);

				if (tag.ContainsKey(nameof(ColorData)))
					pattern = pattern.WithColorData(tag.GetList<PatternColorData>(nameof(ColorData)).ToArray());

				return pattern;
			}
			catch
			{
				string message = "Failed to deserialize pattern:\n";
				message += "\tName: " + (string.IsNullOrEmpty(name) ? "Unknown" : name) + ", ";
				message += "\tModule: " + (string.IsNullOrEmpty(moduleName) ? "Unknown" : moduleName);
				throw new SerializationException(message);
			}	
		}

    }
}
