using Macrocosm.Common.UI;
using Macrocosm.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Navigation.DebugPanel
{
    public class UIRocketDebugPanel : UIPanel
    {
        public Rocket Rocket;

        public UIDynamicTextPanel DebugText;
        public UIListScrollablePanel DebugElementList;

        private UIToggleImage toggleButton;

		public UIRocketDebugPanel()
        {
        }

        string debugString = "";
        public void AddText(string text)
        {
            debugString += text + "\n";
		}

        public override void OnInitialize()
        {
            Top = new Terraria.UI.StyleDimension(0, 0);
            Left = new Terraria.UI.StyleDimension(870, 0);
            Height = new Terraria.UI.StyleDimension(0, 1f);
            Width = new Terraria.UI.StyleDimension(300, 0);

            PaddingTop = 6f;
            PaddingBottom = 6f;
            PaddingLeft = 12f;
            PaddingRight = 12f;

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

            DebugElementList = new("Debug");
            Append(DebugElementList);

			DebugText = new("test")
            {
                PaddingTop = 50f
            };

			toggleButton = new (ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Buttons/Toggle", ReLogic.Content.AssetRequestMode.ImmediateLoad), 32, 32, Point.Zero, new Point(32, 0));
      
			DebugElementList.Append(DebugText);
			DebugElementList.Append(toggleButton);
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Don't delete this or the UIElements attached to this UIState will cease to function
            base.Update(gameTime);

            WorldDataSystem.Instance.FoundVulcan = toggleButton.IsOn;
			DebugText.SetText(debugString);
        }
    }
}
