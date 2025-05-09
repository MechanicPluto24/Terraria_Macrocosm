using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Rockets.LaunchPads;
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
        private readonly RocketModule module;
        private readonly LaunchPad launchPad;
        private readonly List<UIInventorySlot> slots;

        private UIText uITitle;
        private UIHoverImageButton leftArrow;
        private UIHoverImageButton rightArrow;

        public UIModuleAssemblyElement(RocketModule module, LaunchPad launchPad)
        {
            this.module = module;
            this.launchPad = launchPad;

            slots = new();
            if (this.launchPad.TryGetAssemblySlotRangeForModule(module, out Range range))
            {
                (int offset, int length) = range.GetOffsetAndLength(this.launchPad.Inventory.Size);
                for (int i = offset; i < offset + length; i++)
                {
                    UIInventorySlot slot = this.launchPad.Inventory.ProvideItemSlot(i);
                    slots.Add(slot);
                }
            }
        }

        public override void OnInitialize()
        {
            Width.Set(0, 0.4f);
            Height.Set(0, 0.17f);
            SetPadding(6f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            uITitle = new UIText(module.DisplayName, 0.8f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.022f,
                TextColor = Color.White
            };
            Append(uITitle);

            CreateSlots();

            leftArrow = new UIHoverImageButton(
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty)
            {
                Left = new(-4, 0),
                VAlign = 0.5f,
                SpriteEffects = SpriteEffects.FlipHorizontally,
                CheckInteractible = () => launchPad.SwitchAssemblyModuleTier(module, -1, justCheck: true)
            };
            leftArrow.SetVisibility(1f, 0f, 1f);
            leftArrow.OnLeftClick += (_, _) =>
            {
                launchPad.SwitchAssemblyModuleTier(module, -1);

                if (Parent is UIAssemblyTab tab)
                    tab.RefreshAssemblyElements();

            };
            Append(leftArrow);

            rightArrow = new UIHoverImageButton(
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty)
            {
                Left = new(-32, 1f),
                VAlign = 0.5f,
                CheckInteractible = () => launchPad.SwitchAssemblyModuleTier(module, +1, justCheck: true)
            };
            rightArrow.SetVisibility(1f, 0f, 1f);
            rightArrow.OnLeftClick += (_, _) =>
            {
                launchPad.SwitchAssemblyModuleTier(module, +1);

                if (Parent is UIAssemblyTab tab)
                    tab.RefreshAssemblyElements();
            };
            Append(rightArrow);
        }

        private void CreateSlots()
        {
            int slotsPerRow = Math.Min(4, slots.Count);
            int rowCount = (int)Math.Ceiling(slots.Count / 4f);
            float slotSpacing = 0.23f;
            float rowSpacing = 0f;
            for (int i = 0; i < slots.Count; i++)
            {
                UIInventorySlot slot = slots[i];
                AssemblyRecipeEntry recipeEntry = module.Recipe[i];
                if (recipeEntry.RequiredAmount > 1)
                {
                    UIText amountRequiredText = new("x" + recipeEntry.RequiredAmount.ToString(), textScale: 0.8f)
                    {
                        Top = new(0, 0.98f),
                        HAlign = 0.5f
                    };
                    slot.Append(amountRequiredText);
                }

                slot.SizeLimit += 8;
                int row = i / slotsPerRow;
                int column = i % slotsPerRow;
                float rowOffset = (row == 0) ? -rowSpacing / 2f : rowSpacing / 2f;
                float columnOffset = (column - (slotsPerRow - 1) / 2f) * slotSpacing;
                slot.VAlign = 0.5f + rowOffset;
                slot.HAlign = 0.5f + columnOffset;
                Append(slot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var slot in slots)
                slot.CanInteract = Parent != null;
        }

        public void UpdateTitle()
        {
            uITitle.SetText(module.DisplayName.Value);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            module.BlueprintHighlighted = true;
            foreach (var module in launchPad.Rocket.Modules.Where(m => m.Recipe.Linked && m.Recipe.LinkedResult.Name == module.Name))
                module.BlueprintHighlighted = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            module.BlueprintHighlighted = false;
            foreach (var module in launchPad.Rocket.Modules.Where(m => m.Recipe.Linked && m.Recipe.LinkedResult.Name == module.Name))
                module.BlueprintHighlighted = false;
        }
    }
}
