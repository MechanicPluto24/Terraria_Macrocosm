﻿using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Rockets.UI
{
	public class UIPatternIcon : UIPanelIconButton, IFocusable
	{
		public Pattern Pattern { get; set; }


		private Asset<Texture2D> panel;
		public UIPatternIcon(Pattern pattern)
		: base
		(
			Macrocosm.EmptyTex,
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
		)
		{
			Pattern = pattern;
			panel = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad);
        }

        public override void OnInitialize()
		{
			FocusContext = "PatternSelection";
			OnLeftClick += (_, _) => { HasFocus = true; };
			HoverText = Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Customization.Patterns." + Pattern.Name, () => Pattern.Name);
        }

		private SpriteBatchState state;
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			var dimensions = GetOuterDimensions();

			// Load the coloring shader
			Effect effect = ModContent.Request<Effect>(Macrocosm.ShadersPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

			// Pass the pattern icon to the shader via the S1 register
			Main.graphics.GraphicsDevice.Textures[1] = Pattern.IconTexture.Value;

			// Change sampler state for proper alignment at all UI scales 
			SamplerState samplerState = spriteBatch.GraphicsDevice.SamplerStates[1];
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

			//Pass the color mask keys as Vector3s and configured colors as Vector4s
			List<Vector4> colors = new();
			for (int i = 0; i < Pattern.MaxColorCount; i++)
				colors.Add(Pattern.GetColor(i).ToVector4());

			effect.Parameters["uColorCount"].SetValue(Pattern.MaxColorCount);
			effect.Parameters["uColorKey"].SetValue(Pattern.ColorKeys);
			effect.Parameters["uColor"].SetValue(colors.ToArray());
			effect.Parameters["uSampleBrightness"].SetValue(false);

			state.SaveState(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

			spriteBatch.Draw(panel.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(state);

			// Clear the tex registers  
			Main.graphics.GraphicsDevice.Textures[1] = null;

			// Restore the sampler states
			Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
		}
	}
}
