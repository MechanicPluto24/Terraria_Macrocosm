using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIColorMenuHSL : UIPanel
    {
        public enum HSLSliderId
        {
            Hue,
            Saturation,
            Luminance
        }

        public Color PendingColor { get; private set; } = Color.White;
        public Color PreviousColor { get; private set; } = Color.White;

        public float LuminanceSliderFactor { get; init; } = 0.85f;

        private Vector3 currentColorHSL = Color.White.ToHSL();

        private UIText hslText;
        private UIPanel hslTextPanel;

        private UIColoredSlider hueSlider;
        private UIColoredSlider saturationSlider;
        private UIColoredSlider luminanceSlider;

        private UIPanelIconButton copyButton;
        private UIPanelIconButton pasteButton;
        private UIPanelIconButton randomizeButton;

        private UIPanelIconButton applyButton;
        private UIPanelIconButton cancelButton;
        private Action onApplyButtonClicked;
        private Action onCancelButtonClicked;

        public UIColorMenuHSL(float luminanceSliderFactor = 0.85f)
        {
            LuminanceSliderFactor = luminanceSliderFactor;

            Width = new(0f, 0.62f);
            Height = new(0, 0.25f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            SetPadding(6f);

            hueSlider = CreateHSLSlider(HSLSliderId.Hue);
            saturationSlider = CreateHSLSlider(HSLSliderId.Saturation);
            luminanceSlider = CreateHSLSlider(HSLSliderId.Luminance);

            Append(hueSlider);
            Append(saturationSlider);
            Append(luminanceSlider);

            hslTextPanel = new()
            {
                VAlign = 0.94f,
                Width = new(0f, 0.36f),
                Height = new(0f, 0.22f),
                Left = new(0f, 0.03f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            hslTextPanel.SetPadding(2f);

            hslText = new("#FFFFFF", 1f)
            {
                VAlign = 0.45f,
                HAlign = 0.5f
            };

            hslTextPanel.Append(hslText);
            Append(hslTextPanel);

            copyButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"))
            {
                VAlign = 0.93f,
                Left = new(0f, 0.45f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.CopyColorHex")
            };
            copyButton.OnLeftMouseDown += (_, _) => CopyHex();
            Append(copyButton);

            pasteButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"))
            {
                VAlign = 0.93f,
                Left = new(0f, 0.61f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.PasteColorHex")

            };

            pasteButton.OnLeftMouseDown += (_, _) => PasteHex();
            Append(pasteButton);

            randomizeButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"))
            {
                VAlign = 0.93f,
                Left = new(0f, 0.77f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.RandomizeColor")
            };

            randomizeButton.OnLeftMouseDown += (_, _) => RandomizeColor();
            Append(randomizeButton);
        }

        public void SetupApplyAndCancelButtons(Action onApplyButtonClicked, Action onCancelButtonClicked)
        {
            hslTextPanel.Left = new(0f, 0.01f);
            copyButton.Left = new(0f, 0.39f);
            pasteButton.Left = new(0f, 0.51f);
            randomizeButton.Left = new(0f, 0.632f);

            applyButton = new(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Symbols/CheckmarkWhite"))
            {
                VAlign = 0.93f,
                Left = new(0f, 0.88f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.ApplyColor")

            };
            applyButton.OnLeftClick += (_, _) => AcceptChanges();
            this.onApplyButtonClicked = onApplyButtonClicked;
            Append(applyButton);

            cancelButton = new(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Symbols/CrossmarkWhite"))
            {
                VAlign = 0.93f,
                Left = new(0f, 0.76f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.CancelColor")
            };
            cancelButton.OnLeftClick += (_, _) => DiscardChanges();
            this.onCancelButtonClicked = onCancelButtonClicked;

            Append(cancelButton);
        }

        public void SetColorSetEvent(UIElement.MouseEvent mouseEvent)
        {
            hueSlider.OnLeftMouseDown += mouseEvent;
            saturationSlider.OnLeftMouseDown += mouseEvent;
            luminanceSlider.OnLeftMouseDown += mouseEvent;
        }

        public UIColoredSlider CreateHSLSlider(HSLSliderId id)
        {
            UIColoredSlider uIColoredSlider = CreateHSLSliderButtonBase(id);
            uIColoredSlider.Width.Set(0f, 1f);
            uIColoredSlider.Left.Set(0f, -0.15f);
            uIColoredSlider.Top.Set(0f, 0.2f * (int)id);
            uIColoredSlider.SetSnapPoint("Middle", (int)id);
            return uIColoredSlider;
        }

        public UIColoredSlider CreateHSLSliderButtonBase(HSLSliderId id)
        {
            return id switch
            {
                HSLSliderId.Saturation => new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Saturation), delegate (float x)
                {
                    UpdateHSLValue(HSLSliderId.Saturation, x);
                }, UpdateHSL_S, (float x) => GetHSLSliderColorAt(HSLSliderId.Saturation, x), Color.Transparent),

                HSLSliderId.Luminance => new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Luminance), delegate (float x)
                {
                    UpdateHSLValue(HSLSliderId.Luminance, x);
                }, UpdateHSL_L, (float x) => GetHSLSliderColorAt(HSLSliderId.Luminance, x), Color.Transparent),

                _ => new UIColoredSlider(LocalizedText.Empty, () => GetHSLSliderPosition(HSLSliderId.Hue), delegate (float x)
                {
                    UpdateHSLValue(HSLSliderId.Hue, x);
                }, UpdateHSL_H, (float x) => GetHSLSliderColorAt(HSLSliderId.Hue, x), Color.Transparent),
            };
        }

        public void UpdateHSL_H()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(currentColorHSL.X, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Hue, value);
        }

        public void UpdateHSL_S()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(currentColorHSL.Y, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Saturation, value);
        }

        public void UpdateHSL_L()
        {
            float value = UILinksInitializer.HandleSliderHorizontalInput(currentColorHSL.Z, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
            UpdateHSLValue(HSLSliderId.Luminance, value);
        }

        public float GetHSLSliderPosition(HSLSliderId id)
        {
            return id switch
            {
                HSLSliderId.Hue => currentColorHSL.X,
                HSLSliderId.Saturation => currentColorHSL.Y,
                HSLSliderId.Luminance => currentColorHSL.Z,
                _ => 1f,
            };
        }

        public void UpdateHSLValue(HSLSliderId id, float value)
        {
            switch (id)
            {
                case HSLSliderId.Hue:
                    currentColorHSL.X = value;
                    break;
                case HSLSliderId.Saturation:
                    currentColorHSL.Y = value;
                    break;
                case HSLSliderId.Luminance:
                    currentColorHSL.Z = value;
                    break;
            }

            Color color = Utility.ScaledHSLToRGB(currentColorHSL, LuminanceSliderFactor);
            PendingColor = color;

            UpdateHexText(color);
        }

        public Color GetHSLSliderColorAt(HSLSliderId id, float pointAt)
        {
            return id switch
            {
                HSLSliderId.Hue => Utility.ScaledHSLToRGB(pointAt, 1f, 0.5f, LuminanceSliderFactor),
                HSLSliderId.Saturation => Utility.ScaledHSLToRGB(currentColorHSL.X, pointAt, currentColorHSL.Z, LuminanceSliderFactor),
                HSLSliderId.Luminance => Utility.ScaledHSLToRGB(currentColorHSL.X, currentColorHSL.Y, pointAt, LuminanceSliderFactor),
                _ => Color.White,
            };
        }

        public void UpdateKeyboardCapture(Action postCapture = null)
        {
            if (Main.keyState.PressingControl())
            {
                if (Main.keyState.KeyPressed(Keys.C) || Main.keyState.KeyPressed(Keys.Insert))
                {
                    CopyHex();
                    copyButton.TriggerRemoteInteraction();
                }
                else if (Main.keyState.KeyPressed(Keys.V))
                {
                    PasteHex();
                    pasteButton.TriggerRemoteInteraction();
                }
                else if (Main.keyState.KeyPressed(Keys.R))
                {
                    RandomizeColor();
                    randomizeButton.TriggerRemoteInteraction();
                }
            }

            if (postCapture is not null)
                postCapture();
        }

        private void AcceptChanges()
        {
            CaptureCurrentColor();
            onApplyButtonClicked();
        }

        private void DiscardChanges()
        {
            SetColorRGB(PreviousColor);
            onCancelButtonClicked();
        }

        private void CopyHex()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Platform.Get<IClipboard>().Value = hslText.Text;
        }

        private void PasteHex()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string value = Platform.Get<IClipboard>().Value;
            if (Utility.TryGetColorFromHex(value, out var hsl, LuminanceSliderFactor))
            {
                PendingColor = Utility.ScaledHSLToRGB(hsl, LuminanceSliderFactor);
                currentColorHSL = hsl;
                UpdateHexText(Utility.ScaledHSLToRGB(hsl, LuminanceSliderFactor));
            }
        }

        private void RandomizeColor()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            Vector3 randomColorVector = new(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
            PendingColor = Utility.ScaledHSLToRGB(randomColorVector, LuminanceSliderFactor);
            currentColorHSL = randomColorVector;
            UpdateHexText(Utility.ScaledHSLToRGB(randomColorVector, LuminanceSliderFactor));
        }


        public void CaptureCurrentColor()
        {
            PreviousColor = Utility.ScaledHSLToRGB(currentColorHSL, LuminanceSliderFactor);
        }

        public void SetColorRGB(Color rgb)
        {
            PendingColor = rgb;
            currentColorHSL = rgb.ToScaledHSL(LuminanceSliderFactor);
            UpdateHexText(rgb);
        }

        public void SetColorHSL(Vector3 hsl)
        {
            PendingColor = Utility.ScaledHSLToRGB(hsl, LuminanceSliderFactor);
            currentColorHSL = hsl;
            UpdateHexText(Utility.ScaledHSLToRGB(hsl, LuminanceSliderFactor));
        }

        private void UpdateHexText(Color pendingColor)
        {
            hslText.SetText(pendingColor.GetHexText());
        }
    }
}
