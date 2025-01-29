using Macrocosm.Common.Customization;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Consumables.Unlockables;
using Macrocosm.Content.Rockets.Modules;
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
            Rocket visualClone = new(Modules);
            visualClone.ApplyCustomizationChanges(this, sync: false, reset: true);
            return visualClone;
        }

        public bool CheckUnlockableItemUnlocked(Item item)
        {
            if (item.ModItem is PatternDesign patternDesign)
            {
                if (PatternManager.IsUnlocked(patternDesign.PatternName))
                    return true;
            }

            return false;
        }

        public void ApplyCustomizationChanges(Rocket source, bool sync = true, bool reset = true)
        {
            Nameplate.Text = source.Nameplate.Text;
            Nameplate.TextColor = source.Nameplate.TextColor;
            Nameplate.HAlign = source.Nameplate.HAlign;
            Nameplate.VAlign = source.Nameplate.VAlign;

            foreach (var module in Modules)
            {
                module.Decal = source.Modules.FirstOrDefault((m) => m.Name == module.Name).Decal;
                module.Pattern = source.Modules.FirstOrDefault((m) => m.Name == module.Name).Pattern.Clone();
                foreach (var data in module.Pattern.ColorData.Values)
                {
                    if (data.Color.A > 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustDirect(module.Position, module.Width, module.Height, DustID.TintablePaint,
                                newColor: data.Color.WithAlpha(220),
                                Scale: Main.rand.NextFloat(0.2f, 1f));
                        }
                    }
                }
        }

            if (sync)
                SyncCustomizationData();

            if (reset)
                ResetRenderTarget();
        }

        public void ResetCustomizationToDefault()
        {
            Nameplate = new();

            foreach (var module in Modules)
            {
                module.Decal = default;
                module.Pattern = PatternManager.Get("Basic", module.Name);
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
            foreach (var module in Modules)
            {
                modulesArray.Add(new JObject
                {
                    ["moduleName"] = module.Name,
                    ["decal"] = module.Decal.Name,
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
                    var module = Modules.FirstOrDefault((m) => m.Name == moduleName);
                    if (module != null)
                    {
                        try
                        {
                            module.Decal = DecalManager.TryGetDecal(moduleJObject["decal"].Value<string>(), moduleName, out Decal decal) ? decal : new Decal();
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
