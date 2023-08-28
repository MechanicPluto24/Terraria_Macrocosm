using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Macrocosm.Content.Rockets
{
	internal partial class Rocket 
	{
		public Rocket Clone() => DeserializeData(SerializeData());

		/// <summary> Draw the rocket as a dummy </summary>
		public void DrawDummy(SpriteBatch spriteBatch, Vector2 offset, Color drawColor)
		{
			// Passing Rocket world position as "screenPosition" cancels it out  
			Draw(spriteBatch, Position - offset, drawColor);
		}

		public void ApplyCustomizationChanges(Rocket dummy)
		{
			Nameplate.Text = dummy.Nameplate.Text;
			Nameplate.TextColor = dummy.Nameplate.TextColor;
			Nameplate.HorizontalAlignment = dummy.Nameplate.HorizontalAlignment;
			Nameplate.VerticalAlignment = dummy.Nameplate.VerticalAlignment;

			foreach(var moduleName in ModuleNames)
			{
				Modules[moduleName].Detail = dummy.Modules[moduleName].Detail;
				Modules[moduleName].Pattern = dummy.Modules[moduleName].Pattern.Clone();
			}
		}

		public void ResetCustomizationToDefault()
		{
			EngineModule.Nameplate = new();

			foreach(var moduleKvp in Modules)
			{
				moduleKvp.Value.Detail = null;
				moduleKvp.Value.Pattern = CustomizationStorage.GetDefaultPattern(moduleKvp.Key);
			}
		}

		public void ResetModuleCustomizationToDefault(string moduleName)
		{
			if(moduleName is "EngineModule")
 				EngineModule.Nameplate = new();

			Modules[moduleName].Detail = null;
			Modules[moduleName].Pattern = CustomizationStorage.GetDefaultPattern(moduleName);
		}

		public string GetCustomizationDataJSON()
		{
			JArray jArray = new();

			foreach (var moduleKvp in Modules)
 				jArray.Add(moduleKvp.Value.GetCustomizationDataToJObject());
 
			return jArray.ToString(Formatting.Indented);
		}

		public void ApplyRocketCustomizationFromJSON(string json)
		{
			foreach (var moduleKvp in Modules)
			{
				var jArray = JArray.Parse(json);
				JObject jObject = jArray.Cast<JObject>().FirstOrDefault(obj => obj["moduleName"].Value<string>() == moduleKvp.Key);
				moduleKvp.Value.ApplyCustomizationDataFromJObject(jObject);
			}
		}
	}
}
