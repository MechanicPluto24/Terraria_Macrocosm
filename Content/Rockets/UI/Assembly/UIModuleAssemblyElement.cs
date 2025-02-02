using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Assembly
{
    public class UIModuleAssemblyElement : UIPanel
    {
        public RocketModule Module { get; private set; }
        private readonly List<UIInventorySlot> slots;
        private readonly List<RocketModule> linkedModules;

        public Action<UIModuleAssemblyElement, int> OnSwitchModule { get; set; }

        private UIText uITitle;
        private UIHoverImageButton leftArrow;
        private UIHoverImageButton rightArrow;

        public UIModuleAssemblyElement(RocketModule module, List<UIInventorySlot> slots)
        {
            Module = module;
            this.slots = slots;

            linkedModules = new();
        }

        public bool Check(bool consume = false) => Module.Recipe.Check(consume, slots.Select(slot => slot.Item).ToArray());

        public void AddLinked(RocketModule linked)
        {
            linkedModules.Add(linked);
        }

        public override void OnInitialize()
        {
            Width.Set(0, 0.4f);
            Height.Set(0, 0.17f);
            SetPadding(12f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            uITitle = new UIText(Module.DisplayName, 0.8f, false)
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

                slot.HAlign = (slots.Count % 5) switch
                {
                    1 => 0.475f,
                    2 => 0.345f,
                    3 => 0.225f,
                    4 => 0.1f,
                    _ => 0f
                };
                slot.Left = new(0, 0.2f * i);
                Append(slot);
            }

            leftArrow = new UIHoverImageButton(
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty)
            {
                Left = new(0, -0.06f),
                VAlign = 0.5f,
                SpriteEffects = SpriteEffects.FlipHorizontally,

                CheckInteractible = () => !IsRocketActive() && CanSwitchToPreviousModule()
            };
            leftArrow.OnLeftClick += (evt, element) =>
            {
                if (!IsRocketActive() && CanSwitchToPreviousModule())
                    OnSwitchModule?.Invoke(this, -1);
            };
            Append(leftArrow);

            rightArrow = new UIHoverImageButton(
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty)
            {
                Left = new(0, 0.88f),
                VAlign = 0.5f,
                CheckInteractible = () => !IsRocketActive() && CanSwitchToNextModule()
            };
            rightArrow.OnLeftClick += (evt, element) =>
            {
                if (!IsRocketActive() && CanSwitchToNextModule())
                    OnSwitchModule?.Invoke(this, +1);
            };
            Append(rightArrow);
        }

        private bool IsRocketActive() => Module.Rocket?.Active ?? false;

        private bool CanSwitchToPreviousModule()
        {
            var modules = RocketModule.Templates.Where(m => m.Slot == Module.Slot && (m.Configuration == Module.Configuration || m.Configuration == 0 || Module.Configuration == 0)) .OrderBy(m => m.Tier).ToList();
            int index = modules.FindIndex(m => m.Name == Module.Name);
            return index > 0;
        }

        private bool CanSwitchToNextModule()
        {
            var modules = RocketModule.Templates.Where(m => m.Slot == Module.Slot && (m.Configuration == Module.Configuration || m.Configuration == 0 || Module.Configuration == 0)).OrderBy(m => m.Tier).ToList();
            int index = modules.FindIndex(m => m.Name == Module.Name);
            return index < modules.Count - 1;
        }

        public void UpdateTitle()
        {
            uITitle.SetText(Module.DisplayName.Value);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            Module.BlueprintHighlighted = true;
            foreach (var module in linkedModules)
                module.BlueprintHighlighted = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            Module.BlueprintHighlighted = false;
            foreach (var module in linkedModules)
                module.BlueprintHighlighted = false;
        }
    }
}
