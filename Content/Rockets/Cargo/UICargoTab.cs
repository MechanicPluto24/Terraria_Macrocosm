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
    public class UICargoTab : UIPanel
    {
        public Rocket Rocket;

		public UICargoTab()
		{
			Initialize();
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(2f);

			BackgroundColor = new Color(13, 23, 59, 127);
			BorderColor = new Color(15, 15, 15, 255);

			Append(new UIText("test2")
			{
				Top = new(0, 0.5f),
				Left = new(0, 0.5f),
				Width = new(0, 0.2f),
				Height = new(0, 0.2f)
			});

		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
		}
	}
}
