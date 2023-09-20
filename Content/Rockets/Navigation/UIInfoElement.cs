using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
	public class UIInfoElement : UIPanel
    {
		public Action<Vector2> ExtraDraw { get; set; }

        private readonly Asset<Texture2D> icon;

		private readonly LocalizedColorScaleText displayText;
        private readonly LocalizedText hoverText;

        public UIInfoElement(LocalizedColorScaleText displayText, Asset<Texture2D> icon = null, LocalizedText hoverText = null)
        {
            this.icon = icon ?? Macrocosm.EmptyTexAsset; 
            this.displayText = displayText;

            if (hoverText is null)
                this.hoverText = LocalizedText.Empty;
            else
                this.hoverText = hoverText;
        }

        public UIInfoElement(LocalizedText displayText, Asset<Texture2D> icon = null, LocalizedText hoverText = null) : this(new LocalizedColorScaleText(displayText), icon, hoverText)
        {
        }

		public UIInfoElement(string displayText, Asset<Texture2D> icon = null, LocalizedText hoverText = null) : this(Language.GetText(displayText), icon, hoverText)
		{
		}

		public override void OnInitialize()
        {
            Width.Set(0f, 1f);
            Height.Set(40f, 0f);

            BackgroundColor = new Color(43, 56, 101);
            BorderColor = BackgroundColor * 2f;

            UIText uIDisplayText = displayText.ProvideUI();
            uIDisplayText.Left = new StyleDimension(40, 0);
            uIDisplayText.VAlign = 0.5f;
 
            Append(uIDisplayText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
                Main.instance.MouseText(hoverText.Value);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();
            Vector2 iconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.1f, dimensions.Height / 2f);
			spriteBatch.Draw(icon.Value, iconPosition, null, Color.White, 0f, new Vector2(icon.Width() * 0.5f, icon.Height() * 0.5f), 1f, SpriteEffects.None, 0);

            if (ExtraDraw is not null)
                ExtraDraw(iconPosition);
        }
	}
}
