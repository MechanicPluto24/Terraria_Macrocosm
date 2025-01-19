using Macrocosm.Common.Customization;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Consumables.Unlockables;
using Macrocosm.Content.Rockets.Customization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public Rocket VisualClone()
        {
            Rocket visualClone = new(ActiveModuleNames);
            visualClone.ApplyCustomizationChanges(this, sync: false, reset: true);
            return visualClone;
        }

        public bool CheckUnlockableItemUnlocked(Item item)
        {
            if (item.ModItem is PatternDesign patternDesign)
            {
                foreach (var (moduleName, patternName) in patternDesign.Patterns)
                {
                    if (PatternManager.IsUnlocked(moduleName, patternName))
                        return true;
                }
            }

            return false;
        }

        public void ApplyCustomizationChanges(Rocket source, bool sync = true, bool reset = true)
        {
            Nameplate.Text = source.Nameplate.Text;
            Nameplate.TextColor = source.Nameplate.TextColor;
            Nameplate.HAlign = source.Nameplate.HAlign;
            Nameplate.VAlign = source.Nameplate.VAlign;

            foreach (var module in AvailableModules)
            {
                module.Detail = source.AvailableModules.FirstOrDefault((m) => m.Name == module.Name).Detail;
                module.Pattern = source.AvailableModules.FirstOrDefault((m) => m.Name == module.Name).Pattern;

                foreach (PatternColorData data in module.Pattern.ColorData)
                    if (data.Color.A > 0)
                        for (int i = 0; i < 20; i++)
                            Dust.NewDustDirect(module.Position, module.Width, module.Height, DustID.TintablePaint, newColor: data.Color.WithAlpha(220), Scale: Main.rand.NextFloat(0.2f, 1f));
            }

            if (sync)
                SyncCustomizationData();

            if (reset)
                ResetRenderTarget();
        }

        public void ResetCustomizationToDefault()
        {
            Nameplate = new();

            foreach (var module in AvailableModules)
            {
                module.Detail = default;
                module.Pattern = default;
            }

            SyncCustomizationData();
        }

        public string GetCustomizationDataToJSON()
        {
            var jObject = new JObject
            {
                ["nameplate"] = Nameplate.ToJObject()
            };

            var modulesArray = new JArray();
            foreach (var module in AvailableModules)
            {
                modulesArray.Add(new JObject
                {
                    ["moduleName"] = module.Name,
                    ["detail"] = module.Detail.Name,
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
                    var module = AvailableModules.FirstOrDefault((m) => m.Name == moduleName);
                    if (module != null)
                    {
                        try
                        {
                            module.Detail = CustomizationStorage.TryGetDetail(moduleName, moduleJObject["detail"].Value<string>(), out Detail detail) ? detail : new Detail();
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

            ResetRenderTarget();
        }
    }
}
