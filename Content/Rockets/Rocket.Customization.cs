using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Terraria;

namespace Macrocosm.Content.Rockets
{
	public partial class Rocket 
	{
		public Rocket Clone() => DeserializeData(SerializeData());
	
		public void ApplyCustomizationChanges(Rocket dummy)
		{
			Nameplate.Text = dummy.Nameplate.Text;
			Nameplate.TextColor = dummy.Nameplate.TextColor;
			Nameplate.HAlign = dummy.Nameplate.HAlign;
			Nameplate.VAlign = dummy.Nameplate.VAlign;

			foreach(var moduleName in ModuleNames)
			{
				//Modules[moduleName].Detail = dummy.Modules[moduleName].Detail ;
				Modules[moduleName].Pattern = dummy.Modules[moduleName].Pattern;
			}

			SendCustomizationData();
		}

		public void ResetCustomizationToDefault()
		{
			Nameplate = new();

			foreach(var moduleKvp in Modules)
			{
				moduleKvp.Value.Detail = null;
				moduleKvp.Value.Pattern = CustomizationStorage.GetDefaultPattern(moduleKvp.Key);
			}

			SendCustomizationData();
		}

		public string GetCustomizationDataToJSON()
		{
			JArray jArray = new()
			{
				//Nameplate.ToJObject()
			};

			foreach (var moduleKvp in Modules)
			{
				var module = moduleKvp.Value;

				jArray.Add(new JObject()
				{
					// TODO: add detail 
					["moduleName"] = module.Name,
					["pattern"] = module.Pattern.ToJObject()
				});
			}

			return jArray.ToString(Formatting.Indented);
		}

		public void ApplyRocketCustomizationFromJSON(string json)
		{
			foreach (var moduleKvp in Modules)
			{
				var module = moduleKvp.Value;
				var jArray = JArray.Parse(json);
				JObject jObject = jArray.Cast<JObject>().FirstOrDefault(obj => obj["moduleName"].Value<string>() == moduleKvp.Key);

				try
				{
					// TODO: read detail 
					module.Pattern = Pattern.FromJObject(jObject["pattern"].Value<JObject>());
				}
				catch (Exception ex)
				{
					Utility.Chat(ex.Message);
					Macrocosm.Instance.Logger.Warn(ex.Message);
				}

				// TODO: read nameplate 
			}
		}
	}
}
