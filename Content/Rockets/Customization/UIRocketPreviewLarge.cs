using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UIRocketPreviewLarge : UIPanel
    {
        public Rocket Rocket;

		public UIRocketPreviewLarge()
		{
		}

		public override void OnInitialize()
        {
            Width.Set(0, 0.4f);
            Height.Set(0, 1f);
            HAlign = 0f;
            Left.Set(0, 0.6f);
            BackgroundColor = new Color(53, 72, 135);
            BorderColor = new Color(89, 116, 213, 255);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            base.Draw(spriteBatch);

            Rocket.DrawDummy(spriteBatch, GetDimensions().Center() - Rocket.Size / 2f, Color.White);
			
		}
	}
}
