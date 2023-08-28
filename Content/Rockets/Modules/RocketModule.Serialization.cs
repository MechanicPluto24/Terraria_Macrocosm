using System;
using Terraria.ModLoader.IO;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Common.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Macrocosm.Content.Rockets.Modules
{
    internal abstract partial class RocketModule : TagSerializable
	{
		protected virtual JObject SerializeCustomizationData() { return new JObject(); }
		protected virtual void DeserializeCustomizationData(string json) { }

		public string GetCustomizationDataJSON() => GetCustomizationDataToJObject().ToString(Formatting.Indented);

		public JObject GetCustomizationDataToJObject()
		{
			JObject jsonObject = new()
			{
				["moduleName"] = Name,
				["pattern"] = Pattern.ToJObject()
			};

			//jsonObject.Add(SerializeCustomizationData());

			return jsonObject;
		}

		public void ApplyCustomizationDataFromJObject(JObject jObject)
		{
			try
			{
				Pattern = Pattern.FromJObject(jObject["pattern"].Value<JObject>());
			}
			catch (Exception ex)
			{
				Utility.Chat(ex.Message);
				Macrocosm.Instance.Logger.Warn(ex.Message);
			}
			//DeserializeCustomizationData(json);
		}

		public void ApplyCustomizationDataFromJSON(string json) 
		{
			ApplyCustomizationDataFromJObject(JObject.Parse(json));
		}

		protected virtual TagCompound SerializeModuleData() { return new TagCompound(); }
		protected virtual void DeserializeModuleData(TagCompound tag) { }  


		public static readonly Func<TagCompound, RocketModule> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = SerializeModuleData();

			tag["Type"] = FullName;
			tag["Name"] = Name;

			if(Detail is not null)
				tag["DetailName"] = Detail.Name;

			if(Pattern is not null)
				tag["Pattern"] = Pattern;

			return tag;
		}

		public static RocketModule DeserializeData(TagCompound tag)
		{
			string type = tag.GetString("Type");
			string name = tag.GetString("Name");

			RocketModule module = Activator.CreateInstance(Type.GetType(type)) as RocketModule;
			module.DeserializeModuleData(tag);

			if (tag.ContainsKey("DetailName"))
				module.Detail = CustomizationStorage.GetDetail(name, tag.GetString("DetailName"));

			if (tag.ContainsKey("Pattern"))
				module.Pattern = tag.Get<Pattern>("Pattern");

			return module;
		}
	}
}
