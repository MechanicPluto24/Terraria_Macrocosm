using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
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
    public class UICustomizationPreview : UIPanel, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; } 

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
            BackgroundColor = new Color(53, 72, 135);
            BorderColor = new Color(89, 116, 213, 255);

            uITitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"), 0.8f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.005f,
                TextColor = Color.White
            };

            Append(uITitle);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			if (IsMouseHovering)
			{
				BorderColor = Color.Gold;
				BackgroundColor = new Color(53, 72, 135) * 0.9f;
			}
			else
			{
				BorderColor = new Color(89, 116, 213, 255);
				BackgroundColor = new Color(53, 72, 135);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            base.Draw(spriteBatch);

            // Why is it null when resetting?
            if (Rocket is null)
                return;

            Rectangle rect = GetDimensions().ToRectangle();

			spriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			var originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
			RenderTarget2D renderTarget = new(spriteBatch.GraphicsDevice, (int)(Rocket.Bounds.Width * Main.UIScale) , (int)(Rocket.Bounds.Height * Main.UIScale));

            var state = spriteBatch.SaveState();
			spriteBatch.End();

			spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
			spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, Main.UIScaleMatrix);

            Rocket.DrawDummy(spriteBatch, new Vector2(0, 0), Color.White);

			spriteBatch.End();

			spriteBatch.GraphicsDevice.SetRenderTargets(originalRenderTargets);
			spriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;

			Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "Pixelate", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uPixelCount"].SetValue(new Vector2(renderTarget.Width, renderTarget.Height) / (6f * Main.UIScale));

			spriteBatch.Begin(state.SpriteSortMode, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

			Rectangle dest = rect;
			dest.X += 10;
			dest.Y += 27;
			dest.Width -= 20;
			dest.Height -= 34;
			spriteBatch.Draw(renderTarget, dest, Color.White);

			spriteBatch.End();
            spriteBatch.Begin(state);

			renderTarget.Dispose();
		}
	}
}
