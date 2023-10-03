using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
	public partial class Nameplate : TagSerializable
    {
		public string ToJSON() => ToJObject().ToString(Formatting.Indented);

		public JObject ToJObject()
		{
			return new JObject()
			{
				["text"] = text,
				["textColor"] = TextColor.GetHex(),
				["horizontalAlignment"] = HAlign.ToString(),
				["verticalAlignment"] = VAlign.ToString()
			};
		}

		public static Nameplate FromJSON(string json) => FromJObject(JObject.Parse(json));

		public static Nameplate FromJObject(JObject jObject)
		{
			Nameplate nameplate = new();

			if (jObject.ContainsKey("text"))
				nameplate.Text = jObject.Value<string>("text");

			if (jObject.ContainsKey("textColor") && Utility.TryGetColorFromHex(jObject.Value<string>("textColor"), out Color textColor))
 				nameplate.TextColor = textColor;
 
			if (jObject.ContainsKey("horizontalAlignment") && Enum.TryParse(jObject.Value<string>("horizontalAlignment"), ignoreCase: true, out TextHorizontalAlign hAlign))
				nameplate.HAlign = hAlign;

			if (jObject.ContainsKey("verticalAlignment") && Enum.TryParse(jObject.Value<string>("verticalAlignment"), ignoreCase: true, out TextVerticalAlign vAlign))
				nameplate.VAlign = vAlign;

			return nameplate;
		}

		public static readonly Func<TagCompound, Nameplate> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			return new()
			{
				[nameof(text)] = text,
				[nameof(TextColor)] = TextColor,
				[nameof(HAlign)] = (int)HAlign,
				[nameof(VAlign)] = (int)VAlign,
			};
		}

        public static Nameplate DeserializeData(TagCompound tag)
        {
            Nameplate nameplate = new();

			if (tag.ContainsKey(nameof(text)))
				nameplate.text = tag.GetString(nameof(text));

			if (tag.ContainsKey(nameof(TextColor)))
				nameplate.TextColor = tag.Get<Color>(nameof(TextColor));

			if (tag.ContainsKey(nameof(HAlign)))
				nameplate.HAlign = (TextHorizontalAlign)tag.GetInt(nameof(HAlign));

			if (tag.ContainsKey(nameof(VAlign)))
				nameplate.VAlign = (TextVerticalAlign)tag.GetInt(nameof(VAlign));

            return nameplate;
		}
	}
}
