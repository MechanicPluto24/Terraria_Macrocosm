using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Assembly
{
    public class UIModuleAssemblyElement : UIPanel
    {
        public RocketModule Module { get; set; }
        public List<RocketModule> LinkedModules { get; set; } = new();

        private List<UIInventorySlot> slots;
        private UIText uITitle;

        public UIModuleAssemblyElement(RocketModule module, List<UIInventorySlot> slots)
        {
            Module = module;
            this.slots = slots;
        }

        public override void OnInitialize()
        {
            Width.Set(0, 0.361f);
            //Height.Set(0, 0.18f + 0.06f * ((slots.Count - 1) / 4));
            Height.Set(0, 0.17f);
            SetPadding(12f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            uITitle = new(Module.DisplayName, 0.8f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.005f,
                TextColor = Color.White
            };

            Append(uITitle);

            for (int i = 0; i < slots.Count; i++)
            {
                UIInventorySlot slot = slots[i];
                slot.VAlign = 0.5f;
                slot.HAlign = (slots.Count % 4) switch
                {
                    1 => 0.5f,
                    2 => 0.3f,
                    3 => 0.16f,
                    4 => 0f,
                    _ => 0f
                };
                slot.Left = new(0, 0.25f * i);

                Append(slot);
            }
        }

        public bool Check(bool consume = false) => Module.Recipe.Check(consume, slots.Select((slot) => slot.Item).ToArray());

        public override void MouseOver(UIMouseEvent evt)
        {
            Module.BlueprintHighlighted = true;

            foreach (var module in LinkedModules)
                module.BlueprintHighlighted = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            Module.BlueprintHighlighted = false;

            foreach (var module in LinkedModules)
                module.BlueprintHighlighted = false;
        }
    }
}
