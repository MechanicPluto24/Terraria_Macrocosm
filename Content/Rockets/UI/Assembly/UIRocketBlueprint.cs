﻿using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

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
            Width = new(0, 0.5f);
            Height = new(0, 1f);
            Left = new(0, 0.5f);
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

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Rocket == null)
                return;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Center() - Rocket.Bounds.Size() / 2f;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state);
            if (!Rocket.Active)
            {
                Rocket.Draw(Rocket.DrawMode.Blueprint, spriteBatch, position, useRenderTarget: false);
            }
            else
            {
                Rocket.Draw(Rocket.DrawMode.Dummy, spriteBatch, position, useRenderTarget: false);
            }
            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
