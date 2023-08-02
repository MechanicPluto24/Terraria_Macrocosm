using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.OS;
using Terraria.Audio;
using Terraria.ID;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.UI
{
	public class UIColorHSLProvider
	{
		public enum HSLSliderId
		{
			Hue,
			Saturation,
			Luminance
		}

		public Color PendingColor { get; private set; }

		private Vector3 currentColorHSL;

		private UIText hslText; 
		private UIElement container;

		private UIColoredSlider hueSlider;
		private UIColoredSlider saturationSlider;
		private UIColoredSlider luminanceSlider;

		private UIElement copyHexButton;
		private UIElement pasteHexButton;
		private UIElement randomColorButton;

		public UIPanel ProvideHSLMenu()
		{
			UIPanel container = new()
			{
				Width = new(0f, 0.485f),
				Height = new(0, 0.25f),
				HAlign = 0.98f,
				Top = new(0f, 0.095f),
				BackgroundColor = new Color(53, 72, 135),
			    BorderColor = new Color(89, 116, 213, 255)
			};

			container.SetPadding(10f);

			hueSlider = CreateHSLSlider(HSLSliderId.Hue);
			saturationSlider = CreateHSLSlider(HSLSliderId.Saturation);
			luminanceSlider = CreateHSLSlider(HSLSliderId.Luminance);

			container.Append(hueSlider);
			container.Append(saturationSlider);
			container.Append(luminanceSlider);
			UIPanel uIPanel = new()
			{
				VAlign = 1f,
				HAlign = 1f,
				Width = StyleDimension.FromPixelsAndPercent(0f, 0.4f),
				Height = StyleDimension.FromPixelsAndPercent(0f, 0.235f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};

			hslText = new("#FFFFFF", 0.9f)
			{
				VAlign = 0.5f,
				HAlign = 0.5f
			};

			uIPanel.Append(hslText);
			container.Append(uIPanel);

			UIColoredImageButton copyButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(0f, 0f)
			};

			copyButton.OnLeftMouseDown += Click_CopyHex;
			container.Append(copyButton);
			copyHexButton = copyButton;

			UIColoredImageButton pasteButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(40f, 0f)
			};

			pasteButton.OnLeftMouseDown += Click_PasteHex;
			container.Append(pasteButton);
			pasteHexButton = pasteButton;

			UIColoredImageButton randomizeButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), isSmall: true)
			{
				VAlign = 1f,
				HAlign = 0f,
				Left = StyleDimension.FromPixelsAndPercent(80f, 0f)
			};

			randomizeButton.OnLeftMouseDown += Click_RandomizeSingleColor;
			container.Append(randomizeButton);
			randomColorButton = randomizeButton;

			copyButton.SetSnapPoint("Low", 0);
			pasteButton.SetSnapPoint("Low", 1);
			randomizeButton.SetSnapPoint("Low", 2);

			return container;
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
			uIColoredSlider.Width = StyleDimension.FromPixelsAndPercent(0f, 1f);
			uIColoredSlider.Left.Set(-10f, 0f);
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

			Color color = Utility.HSLToRGB(currentColorHSL);
			PendingColor = color;

			UpdateHexText(color);
		}

		public Color GetHSLSliderColorAt(HSLSliderId id, float pointAt)
		{
			return id switch
			{
				HSLSliderId.Hue => Utility.HSLToRGB(pointAt, 1f, 0.5f),
				HSLSliderId.Saturation => Utility.HSLToRGB(currentColorHSL.X, pointAt, currentColorHSL.Z),
				HSLSliderId.Luminance => Utility.HSLToRGB(currentColorHSL.X, currentColorHSL.Y, pointAt),
				_ => Color.White,
			};
		}


		private void Click_CopyHex(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Platform.Get<IClipboard>().Value = hslText.Text;
		}

		private void Click_PasteHex(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			string value = Platform.Get<IClipboard>().Value;
			if (Utility.TryGetColorFromHex(value, out var hsl))
			{
				PendingColor = Utility.HSLToRGB(hsl);
				currentColorHSL = hsl;
				UpdateHexText(Utility.HSLToRGB(hsl));
			}
		}

		public void SetColorHSL(Vector3 hsl)
		{
			PendingColor = Utility.HSLToRGB(hsl);
			currentColorHSL = hsl;
			UpdateHexText(Utility.HSLToRGB(hsl));
		}

		private void Click_RandomizeSingleColor(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);			
			Vector3 randomColorVector = new(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
			PendingColor = Utility.HSLToRGB(randomColorVector);
			currentColorHSL = randomColorVector;
			UpdateHexText(Utility.HSLToRGB(randomColorVector));
		}

		private void UpdateHexText(Color pendingColor)
		{
			hslText.SetText(pendingColor.GetHexText());
		}
	}
}
