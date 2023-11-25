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

            foreach (var moduleName in ModuleNames)
            {
                //Modules[moduleName].Detail = dummy.Modules[moduleName].Detail ;
                Modules[moduleName].Pattern = dummy.Modules[moduleName].Pattern;
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
