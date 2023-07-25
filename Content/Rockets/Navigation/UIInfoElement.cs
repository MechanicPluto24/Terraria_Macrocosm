using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Common.UI;
using Terraria.Localization;
using ReLogic.Content;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UIInfoElement : UIPanel
    {
        private readonly Asset<Texture2D> icon;

        private readonly LocalizedColorScaleText displayText;
        private readonly LocalizedText hoverText;

        public UIInfoElement(Asset<Texture2D> icon, LocalizedColorScaleText displayText, LocalizedText hoverText = null)
        {
            this.icon = icon;
            this.displayText = displayText;

            if (hoverText is null)
                this.hoverText = LocalizedText.Empty;
            else
                this.hoverText = hoverText;
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
            spriteBatch.Draw(icon.Value, dimensions.Position() + new Vector2(dimensions.Width * 0.1f, dimensions.Height / 2f), null, Color.White, 0f, new Vector2(icon.Width() * 0.5f, icon.Height() * 0.5f), 1f, SpriteEffects.None, 0);
        }
	}
}
