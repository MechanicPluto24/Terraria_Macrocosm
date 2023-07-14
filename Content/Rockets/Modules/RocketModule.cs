using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract class RocketModule 
	{
		public string Name => GetType().Name;

		public Vector2 Position { get; set; }
		public Vector2 Center {
			get => Position + Size / 2f;
			set => Position = value - Size / 2f;
		}

		public virtual int Width => Texture.Width;
		public virtual int Height => Texture.Height;

		public Vector2 Size => new(Width, Height);
		public Rectangle Hitbox => new((int)Position.X, (int)Position.Y, Width, Height);

		protected Vector2 Origin => new(0, 0);


		public virtual string TexturePath => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');
		public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

		public Detail Detail { get; set; }
		public Pattern Pattern { get; set; }

		public bool HasPattern => Pattern is not null;
		public bool HasDetail => Detail is not null;
		private bool SpecialDraw => HasPattern || HasDetail;

		public RocketModule()
		{
			Pattern = CustomizationStorage.GetPattern(GetType().Name, "Basic");
			//Detail = CustomizationStorage.GetDetail(...)
		}

		public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
		{
			// Load current pattern and apply shader 
			SpriteBatchState state = spriteBatch.SaveState();
			SamplerState samplerState = Main.graphics.GraphicsDevice.SamplerStates[1]; 
			if (SpecialDraw)
			{
				// Load the coloring shader
				Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

				// -- testing, will be configured from an UI		
				if (this is EngineModule)
				{
					Pattern = CustomizationStorage.GetPattern("EngineModule", "Binary");
					//Detail = CustomizationStorage.GetDetail(...)
				}

				if (HasPattern)
				{
					// Pass the pattern to the shader via the S1 register
					Main.graphics.GraphicsDevice.Textures[1] = Pattern.Texture;

					// Change sampler state for proper alignment at all zoom levels 
					Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

					//Pass the color mask keys as Vector3s and configured colors as Vector4s
					//Passing the entire color table (MaxColorCount), not only the configurable color count
					//(parameters are scalars intentionally)
					for(int i = 0; i < Pattern.MaxColorCount; i++)
					{
						effect.Parameters["uColorKey" + i.ToString()].SetValue(Pattern.ColorKeys[i]);
						effect.Parameters["uColor" + i.ToString()].SetValue(Pattern.Colors[i].ToVector4());
					}

					// Get a blend between the general ambient color at the rocket center, and the local color on this module
					Color localColor = Color.Lerp(Lighting.GetColor((int)(Center.X) / 16, (int)(Center.Y) / 16), ambientColor, 0.8f);

					//Pass the ambient lighting on the rocket 
					effect.Parameters["uAmbientColor"].SetValue(localColor.ToVector3());
				}

				if (HasDetail)
				{
					// Pass the detail to the shader via the S2 register
					Main.graphics.GraphicsDevice.Textures[2] = Detail.Texture;
				}

				spriteBatch.End();
				spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);
			}

			spriteBatch.Draw(Texture, Position - screenPos, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			if (SpecialDraw)
			{
				spriteBatch.End();
				spriteBatch.Begin(state);

				// Clear the tex registers  
				Main.graphics.GraphicsDevice.Textures[1] = null;
				Main.graphics.GraphicsDevice.Textures[2] = null;

				// Restore the sampler states
				Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
			}
		}
	}
}
