using Macrocosm.Content.Rockets.Customization;
using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract partial class RocketModule : TagSerializable
    {
        protected virtual TagCompound SerializeModuleSpecificData() { return new TagCompound(); }
        protected virtual void DeserializeModuleSpecificData(TagCompound tag, Rocket ownerRocket) { }

        public static readonly Func<TagCompound, RocketModule> DESERIALIZER = DeserializeData;

        public TagCompound SerializeData()
        {
            TagCompound tag = SerializeModuleSpecificData();

            tag["Type"] = FullName;
            tag["Name"] = Name;

            if (Detail is not null)
                tag["DetailName"] = Detail.Name;

			if(Pattern != default)
				tag["Pattern"] = Pattern;

            return tag;
        }

        /// <summary>
        /// UNUSED! 
        /// Deserialization is customly done using the <see cref="DeserializeData(TagCompound, Rocket)"/> method instead.
        /// Care should be taken since calling tag.Get<RocketModule>() will call this method.
        /// </summary>
        public static RocketModule DeserializeData(TagCompound tag)
        {
            // There must be a way to clean this all up... - Feldy
            throw new NotSupportedException("You should deserialize the module data with DeserializeData(TagCompound, Rocket) instead, by calling it directly");
        }

        public static RocketModule DeserializeData(TagCompound tag, Rocket ownerRocket)
        {
            string type = tag.GetString("Type");
            string name = tag.GetString("Name");

            RocketModule module = Activator.CreateInstance(Type.GetType(type), ownerRocket) as RocketModule;
            module.DeserializeModuleSpecificData(tag, ownerRocket);

            if (tag.ContainsKey("DetailName"))
                module.Detail = CustomizationStorage.GetDetail(name, tag.GetString("DetailName"));

            if (tag.ContainsKey("Pattern"))
                module.Pattern = tag.Get<Pattern>("Pattern");

            return module;
        }
    }
}
