using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {

        public Rocket VisualClone()
        {
            Rocket visualClone = new();
            visualClone.ApplyCustomizationChanges(this);
            return visualClone;
        }

        public void ApplyCustomizationChanges(Rocket source)
        {
            Nameplate.Text = source.Nameplate.Text;
            Nameplate.TextColor = source.Nameplate.TextColor;
            Nameplate.HorizontalAlignment = source.Nameplate.HorizontalAlignment;
            Nameplate.VerticalAlignment = source.Nameplate.VerticalAlignment;

            foreach (var moduleName in ModuleNames)
            {
                Modules[moduleName].Detail = source.Modules[moduleName].Detail;
                Modules[moduleName].Pattern = source.Modules[moduleName].Pattern.Clone();
            }
        }

        public void ResetCustomizationToDefault()
        {
            EngineModule.Nameplate = new();

            foreach (var moduleKvp in Modules)
            {
                moduleKvp.Value.Detail = null;
                moduleKvp.Value.Pattern = CustomizationStorage.GetDefaultPattern(moduleKvp.Key);
            }
        }

        public void ResetModuleCustomizationToDefault(string moduleName)
        {
            if (moduleName is "EngineModule")
                EngineModule.Nameplate = new();

            Modules[moduleName].Detail = null;
            Modules[moduleName].Pattern = CustomizationStorage.GetDefaultPattern(moduleName);
        }

        public string GetCustomizationDataToJSON()
        {
            JArray jArray = new();

            foreach (var moduleKvp in Modules)
            {
                var module = moduleKvp.Value;

                jArray.Add(new JObject()
                {
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
                    module.Pattern = Pattern.FromJObject(jObject["pattern"].Value<JObject>());
                }
                catch (Exception ex)
                {
                    Utility.Chat(ex.Message);
                    Macrocosm.Instance.Logger.Warn(ex.Message);
                }
            }
        }
    }
}
