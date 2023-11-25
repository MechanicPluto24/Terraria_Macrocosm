using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
	public readonly partial struct PatternColorData : TagSerializable
	{
		public TagCompound SerializeData()
		{
			TagCompound tag = new();

			if (HasColorFunction)
			{
				// TODO: TagSerializable for ColorFunction (?)
				tag[nameof(ColorFunction)] = ColorFunction.Name;

				if(ColorFunction.HasParameters)
					tag["parameters"] = ColorFunction.Parameters.ToList();
			}
			else if(IsUserModifiable)
			{
				tag[nameof(Color)] = Color;
			}

			return tag;
		}

		public static readonly Func<TagCompound, PatternColorData> DESERIALIZER = DeserializeData;

		public static PatternColorData DeserializeData(TagCompound tag)
		{
			if(tag.ContainsKey(nameof(ColorFunction)))
			{
				string functionName = tag.GetString(nameof(ColorFunction));

				object[] parameters = Array.Empty<object>();
				if (tag.ContainsKey("parameters"))
					parameters = tag.GetList<object>("parameters").ToArray();

				return new(ColorFunction.CreateByName(functionName, parameters));
 			}

			if (tag.ContainsKey(nameof(Color)))
			{
				return new(tag.Get<Color>(nameof(Color)));  
			}

			return new();
		}
	}
}
