using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.GameContent.TextureAssets;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICustomizationPreview : UIPanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        private Rocket visualClone;

        private UIText uITitle;

        public UICustomizationPreview()
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0, 0.124f);
            Height.Set(0, 0.336f);
            HAlign = 0f;
            Top.Set(0, 0.01f);
            Left.Set(0, 0.01f);
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;

            uITitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"), 0.8f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.005f,
                TextColor = Color.White
            };

            Append(uITitle);
        }

        public void OnTabOpen()
        {
            if (Rocket is not null)
                GetClone();
        }

        private void GetClone()
        {
            visualClone = Rocket.VisualClone();
            visualClone.ForcedStationaryAppearance = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
                BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
            }
            else
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor;
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            }
        }

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (Rocket is null)
                return;

            if (visualClone is null)
                GetClone();

            RenderTarget2D renderTarget = visualClone.GetOrPrepareRenderTarget(Rocket.DrawMode.Dummy);

            Rectangle rect = GetDimensions().ToRectangle();
            rect.X += 10;
            rect.Y += 27;
            rect.Width -= 20;
            rect.Height -= 34;

            Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "Pixelate", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uPixelCount"].SetValue(new Vector2(visualClone.Bounds.Width, visualClone.Bounds.Height));

            state.SaveState(spriteBatch);
            spriteBatch.EndIfBeginCalled();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, state.RasterizerState, effect, Main.UIScaleMatrix);

            spriteBatch.Draw(renderTarget, rect, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
