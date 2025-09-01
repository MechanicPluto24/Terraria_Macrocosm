using Macrocosm.Common.Customization;
using System;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract partial class RocketModule : TagSerializable
    {

        public static readonly Func<TagCompound, RocketModule> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Name)] = Name,
                [nameof(IsBlueprint)] = IsBlueprint
            };

            if (Decal != default) tag[nameof(Decal)] = Decal.Name;
            if (Pattern != null) tag[nameof(Pattern)] = Pattern;
            return tag;
        }

        public static RocketModule DeserializeData(TagCompound tag)
        {
            string name = tag.GetString(nameof(Name));
            RocketModule template = Templates.FirstOrDefault(m => m.Name == name || LegacyNameAttribute.GetLegacyNamesOfType(m.GetType()).Contains(name)) ?? throw new Exception($"RocketModule template not found: {name}");
            if (Activator.CreateInstance(template.GetType()) is not RocketModule module) throw new Exception($"Failed to create instance of RocketModule: {name}");

            module.IsBlueprint = tag.ContainsKey(nameof(IsBlueprint));
            if (tag.ContainsKey(nameof(Decal))) module.Decal = DecalManager.GetDecal(name, tag.GetString(nameof(Decal)));

            if (tag.ContainsKey(nameof(Pattern)))
                module.Pattern = tag.Get<Pattern>(nameof(Pattern));

            return module;
        }
    }
}
