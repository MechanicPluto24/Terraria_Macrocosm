using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UIAssemblyTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        public UIAssemblyTab()
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(3f);

            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.TabStyle.BorderColor;
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            var dims = GetDimensions();
            Rocket.DrawBlueprint(spriteBatch, dims.Center() - Rocket.Bounds.Size() / 2f, UITheme.Current.PanelStyle.BorderColor, UITheme.Current.PanelStyle.BackgroundColor, UITheme.Current.WindowStyle.BackgroundColor * 1.25f, UITheme.Current.ButtonStyle.BackgroundColor);
        }
    }
}
