using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace Macrocosm.Common.UI
{
	internal class UIPatternIcon : UIFocusIconButton, IFocusable
	{
		public Pattern Pattern { get; set; }

		public UIPatternIcon(Pattern pattern)
		: base
		(
			Macrocosm.EmptyTexAsset,
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
			ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
		) 
		{
			Pattern = pattern;
		}

		public override void OnInitialize()
		{
 			FocusContext = "PatternSelection";
			OnLeftClick += (_, _) => { HasFocus = true; };
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			var dimensions = GetOuterDimensions();

			// Load the coloring shader
			Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "SimpleColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

			// Pass the pattern icon to the shader via the S1 register
			Main.graphics.GraphicsDevice.Textures[1] = Pattern.IconTexture;

			// Change sampler state for proper alignment at all UI scales 
			SamplerState samplerState = spriteBatch.GraphicsDevice.SamplerStates[1];
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

			//Pass the color mask keys as Vector3s and configured colors as Vector4s
			//Note: parameters are scalars intentionally, I manually unrolled the loop in the shader to reduce number of branch instructions -- Feldy
			for (int i = 0; i < Pattern.MaxColorCount; i++)
			{
				effect.Parameters["uColorKey" + i.ToString()].SetValue(Pattern.ColorKeys[i]);
				effect.Parameters["uColor" + i.ToString()].SetValue(Pattern.GetColor(i).ToVector4());
			}

			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

			spriteBatch.Draw(Pattern.IconTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(state);

			// Clear the tex registers  
			Main.graphics.GraphicsDevice.Textures[1] = null;

			// Restore the sampler states
			Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
		}
	}
}
