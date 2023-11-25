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
            Nameplate.HAlign = source.Nameplate.HAlign;
            Nameplate.VAlign = source.Nameplate.VAlign;

            foreach (var moduleName in ModuleNames)
            {
                //Modules[moduleName].Detail = dummy.Modules[moduleName].Detail ;
                Modules[moduleName].Pattern = source.Modules[moduleName].Pattern;
            }

            SendCustomizationData();
        }

        public void ResetCustomizationToDefault()
        {
            Nameplate = new();

            foreach (var moduleKvp in Modules)
            {
                moduleKvp.Value.Detail = null;
                moduleKvp.Value.Pattern = CustomizationStorage.GetDefaultPattern(moduleKvp.Key);
            }

            SendCustomizationData();
        }

        public string GetCustomizationDataToJSON()
        {
            var jObject = new JObject
            {
                ["nameplate"] = Nameplate.ToJObject()
            };

            var modulesArray = new JArray();
            foreach (var moduleKvp in Modules)
            {
                var module = moduleKvp.Value;
                modulesArray.Add(new JObject
                {
                    ["moduleName"] = module.Name,
                    ["pattern"] = module.Pattern.ToJObject()
                });
            }

            jObject["modules"] = modulesArray;

            return jObject.ToString(Formatting.Indented);
        }

        public void ApplyRocketCustomizationFromJSON(string json)
        {
            var jObject = JObject.Parse(json);

            if (jObject["nameplate"] is JObject nameplateJObject)
            {
                Nameplate = Nameplate.FromJObject(nameplateJObject);
            }

            if (jObject["modules"] is JArray modulesArray)
            {
                foreach (var moduleJObject in modulesArray.Children<JObject>())
                {
                    string moduleName = moduleJObject["moduleName"].Value<string>();
                    if (Modules.TryGetValue(moduleName, out var module))
                    {
                        try
                        {
                            module.Pattern = Pattern.FromJObject(moduleJObject["pattern"].Value<JObject>());
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
    }
}
