using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Macrocosm.Content.Items.Blocks;
using System;
using System.Linq;
using Macrocosm.Content.Rockets.Modules;
using System.Collections.Generic;

namespace Macrocosm.Content.Rockets.UI.Assembly
{
    public class UIRocketBlueprint : UIPanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        public UIRocketBlueprint()
        {
        }

        public override void OnInitialize()
        {
            Width = new(0, 0.4f);
            Height = new(0, 1f);
            Left = new(0, 0.6f);
            HAlign = 0f;
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
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

            if (Rocket == null)
                return;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Center() - Rocket.Bounds.Size() / 2f;

            if (!Rocket.Active)
                Rocket.Draw(Rocket.DrawMode.Blueprint, spriteBatch, position, useRenderTarget: false);
            else
                Rocket.Draw(Rocket.DrawMode.Dummy, spriteBatch, position, useRenderTarget: true);
        }
    }
}
