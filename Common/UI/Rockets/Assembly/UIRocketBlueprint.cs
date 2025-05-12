using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Common.UI.Rockets.Assembly
{
    public class UIRocketBlueprint : UIPanel
    {
        public Rocket Rocket { get; set; }

        public UIRocketBlueprint()
        {
        }

        public override void OnInitialize()
        {
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
            Vector2 position = dimensions.Center() - Rocket.Bounds.Size / 2f;

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
