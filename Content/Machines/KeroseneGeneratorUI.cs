using Macrocosm.Common.Config;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;


namespace Macrocosm.Content.Machines
{
    public class KeroseneGeneratorUI : MachineUI
    {
        public KeroseneGeneratorTE KeroseneGenerator => MachineTE as KeroseneGeneratorTE;

        private UIPanel backgroundPanel;
        private UIPanel inventoryPanel;
        private UIPanelProgressBar burnItemIconProgressBar;
        private UIPanelProgressBar hullHeatProgressBar;
        private UIInventoryItemIcon itemIcon;
        private UITextPanel<string> hullHeatText;
        private UITextPanel<string> powerStatusText;
        private UIHoverImageButton arrow;

        public KeroseneGeneratorUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(495f, 0f);
            Height.Set(250f, 0f);

            title.Top.Set(-36, 0);

            //Recalculate();

            backgroundPanel = new()
            {
                Width = new(0, 1),
                Height = new(0, 1),
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            Append(backgroundPanel);

            powerStatusText = new("", textScale: 1f, large: false)
            {
                HAlign = 1f,
                VAlign = 0.04f,
                Width = new(0, 0.6f - 0.01f),
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(powerStatusText);

            hullHeatText = new("", textScale: 1f, large: false)
            {
                HAlign = 0f,
                VAlign = 0.04f,
                Width = new(0, 0.4f - 0.01f),
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(hullHeatText);

            hullHeatProgressBar = new()
            {
                Width = new(0, 1f),
                Height = new(42, 0),
                Left = new(0, 0f),
                VAlign = 0.45f,
                FillColor = new Color(255, 255, 0),
                FillColorEnd = new Color(255, 0, 0),
                IsVertical = false,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(hullHeatProgressBar);

            inventoryPanel = new()
            {
                Width = new(0, 1f),
                Height = new(48 + 10, 0),
                Left = new(0, 0f),
                VAlign = 1f,
                PaddingLeft = PaddingRight = 20,
                PaddingTop = PaddingBottom = 10,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(inventoryPanel);

            if (KeroseneGenerator.Inventory is not null)
            {
                for (int i = 0; i < KeroseneGenerator.Inventory.Size; i++)
                {
                    var inputSlot = KeroseneGenerator.Inventory.ProvideItemSlot(i, ItemSlot.Context.ChestItem);

                    inputSlot.Left = new(i * 48, 0f);
                    inputSlot.VAlign = 0.5f;
                    inputSlot.SetPadding(0f);
                    inventoryPanel.Append(inputSlot);
                }
            }

            arrow = new(ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Buttons/LongArrow", AssetRequestMode.ImmediateLoad))
            {
                VAlign = 0.5f,
                HAlign = 0.5f

            };
            arrow.SetVisibility(1f);
            inventoryPanel.Append(arrow);

            burnItemIconProgressBar = new()
            {
                Width = new(46, 0),
                Height = new(46, 0),
                Left = new(0, 0.85f),
                VAlign = 0.95f,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColor = new Color(255, 255, 0),
                FillColorEnd = new Color(255, 0, 0),
                IsVertical = true,
            };
            backgroundPanel.Append(burnItemIconProgressBar);

            itemIcon = new()
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            burnItemIconProgressBar.Append(itemIcon);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Inventory.ActiveInventory = KeroseneGenerator.Inventory;

            string power = $"{KeroseneGenerator.GeneratedPower:F2}";
            powerStatusText.SetText(Language.GetText("Mods.Macrocosm.Machines.Common.GeneratedPower").Format(power));

            burnItemIconProgressBar.Progress = KeroseneGenerator.BurnProgress;

            hullHeatProgressBar.Progress = KeroseneGenerator.HullHeatProgress;

            float temperature = MacrocosmSubworld.GetCurrentAmbientTemperature() + KeroseneGenerator.HullHeat;
            if (ClientConfig.Instance.UnitSystem is ClientConfig.UnitSystemType.Metric)
                hullHeatText.SetText(Language.GetText("Mods.Macrocosm.Machines.KeroseneGenerator.HullHeatMetric").Format((int)temperature));
            else if (ClientConfig.Instance.UnitSystem is ClientConfig.UnitSystemType.Imperial)
                hullHeatText.SetText(Language.GetText("Mods.Macrocosm.Machines.KeroseneGenerator.HullHeatImperial").Format((int)Utility.CelsiusToFarhenheit(temperature)));

            if (itemIcon.Item.type != KeroseneGenerator.ConsumedItem.type)
                itemIcon.Item = KeroseneGenerator.ConsumedItem;
        }
    }
}
