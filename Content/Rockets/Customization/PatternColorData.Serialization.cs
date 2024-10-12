using Microsoft.Xna.Framework;
using System;
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

                if (ColorFunction.HasParameters)
                    tag["parameters"] = string.Join(", ", ColorFunction.Parameters.Select(p => p.ToString()));
            }
            else if (IsUserModifiable)
            {
                tag[nameof(Color)] = Color;
            }

            return tag;
        }

        public static readonly Func<TagCompound, PatternColorData> DESERIALIZER = DeserializeData;

        public static PatternColorData DeserializeData(TagCompound tag)
        {
            if (tag.ContainsKey(nameof(ColorFunction)))
            {
                string functionName = tag.GetString(nameof(ColorFunction));

                string[] parameters = Array.Empty<string>();
                if (tag.ContainsKey("parameters"))
                    parameters = tag.GetString("parameters").Split(new[] { ", " }, StringSplitOptions.None);

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
