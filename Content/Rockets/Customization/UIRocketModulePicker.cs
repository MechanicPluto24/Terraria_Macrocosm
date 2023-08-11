using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Macrocosm.Common.UI;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UIRocketModulePicker : UIPanel
	{
		public Rocket Rocket;

		public UIHoverImageButton RightButton;
		public UIHoverImageButton LeftButton;

		public string CurrentModuleName = "CommandPod";

		private UIText uITitle;

		private UIPanel tempPanel;

		public UIRocketModulePicker()
		{
		}

		public override void OnInitialize()
		{
			var mode = ReLogic.Content.AssetRequestMode.ImmediateLoad;
			string buttonsPath = "Macrocosm/Content/Rockets/Textures/Buttons/";

			Width.Set(0, 0.485f);
			Height.Set(0, 0.25f); 
			HAlign = 0.007f;
			Top.Set(0f, 0.095f);
			BackgroundColor = new Color(53, 72, 135);
			BorderColor = new Color(89, 116, 213, 255);
			SetPadding(0f);

			uITitle = new(Language.GetText("Mods.Macrocosm.RocketUI.Modules." + CurrentModuleName), 0.9f, false)
			{
				IsWrapped = false,
				HAlign = 0.5f,
				VAlign = 0.05f,
				TextColor = Color.White
			};

			Append(uITitle);

			tempPanel = new()
			{
				Width = new(0, 0.5f),
				Height = new(0, 0.75f),
				HAlign = 0.5f,
				VAlign = 0.8f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			Append(tempPanel);

			LeftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode))
			{
				VAlign = 0.55f,
				Left = new(0, 0.12f),
			};
			LeftButton.SetVisibility(1f, 1f, 1f);
			Append(LeftButton);

			RightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode))
			{
				VAlign = 0.55f,
				Left = new(0, 0.725f),
			};
			RightButton.SetVisibility(1f, 1f, 1f);
			Append(RightButton);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			uITitle.SetText(Language.GetText("Mods.Macrocosm.RocketUI.Modules." + CurrentModuleName));
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
