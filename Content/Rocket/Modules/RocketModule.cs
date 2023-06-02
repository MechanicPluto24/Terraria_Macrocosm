using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using System;

namespace Macrocosm.Content.Rocket.Modules
{
	public abstract class RocketModule
	{
		public virtual string TexturePath => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

		public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

		public List<string> PatternPaths;
		public string PatternName = "Rainbow";

		public Texture2D PatternTexture
		{
			get {
				if (ModContent.RequestIfExists(TexturePath + "_Pattern_" + PatternName, out Asset<Texture2D> paintMask))
					return paintMask.Value;
				else
					return null;
			}
		}

		public Vector2 Position { get; set; }
		protected Vector2 Origin => new(Texture.Width / 2, 0);

		public bool HasPattern => PatternTexture is not null;

		public Color[] Colors = new Color[8];
 

		public virtual void Draw(SpriteBatch spriteBatch, Color ambientColor)
		{
			PatternName = "Basic"; // -- testing, will be configured from an UI

			Array.Fill(Colors, Color.White);

			// Load current pattern and apply shader 
			SpriteBatchState state = spriteBatch.SaveState(); 
			if (HasPattern)
			{
				if(this is EngineModule.EngineModule)
				{
					// -- testing, will be configured from an UI		
					PatternName = "Rainbow";  		  
					Colors[0] = Color.White;
					Colors[1] = Color.Red;
					Colors[2] = Color.Orange;
					Colors[3] = Color.Yellow;
					Colors[4] = Color.Green;
					Colors[5] = Color.Blue;
					Colors[6] = Color.Indigo;
					Colors[7] = Color.Violet;
				}

				// Load shader
				Effect effect = ModContent.Request<Effect>("Macrocosm/Assets/Effects/ColorMaskShading",AssetRequestMode.ImmediateLoad).Value;

				// Pass the pattern to the shader via the S1 register
				Main.graphics.GraphicsDevice.Textures[1] = PatternTexture;

				// Configure the SamplerState of the pattern.
				// Needed for proper alignment of texture and coords at some zoom levels
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

				effect.Parameters["uColorNum"].SetValue(8); // will be specified in the Pattern

				//Pass the color mask keys as a Vector3 array
				effect.Parameters["uColorKeys0"].SetValue(ColorKeys0);
				effect.Parameters["uColorKeys1"].SetValue(ColorKeys1);

				// Pass the configured colors as a Vector4 array
				effect.Parameters["uColors0"].SetValue(Colors[0..4].ToVector4Array());
				effect.Parameters["uColors1"].SetValue(Colors[4..8].ToVector4Array());

				//Pass the ambient lighting on the rocket 
				effect.Parameters["uAmbientBrightness"].SetValue(ambientColor.GetLuminance());

				spriteBatch.End();
				spriteBatch.Begin(effect, state);
			}

			spriteBatch.Draw(Texture, Position, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			if (HasPattern)
			{
				spriteBatch.End();
				spriteBatch.Restore(state);
			}
		}

		/// <summary> Color mask keys </summary>
		public static readonly Vector3[] ColorKeys0 = {
			new Vector3(0f, 1f, 1f), // Cyan    - Accent color (for example, the rocket tip)
			new Vector3(1f, 0f, 1f), // Magenta - Background hull color
			new Vector3(1f, 1f, 0f), // Yellow  - Primary hull color
			new Vector3(0f, 1f, 0f)	 // Green   - Secondary hull color  
		};

		/// <summary> Extra color mask keys (for detailed patterns) </summary>
		public static readonly Vector3[] ColorKeys1 = {
			new Vector3(1f, 0f, 0f), // Red
			new Vector3(0f, 0f, 1f), // Blue   
			new Vector3(1f, 1f, 1f), // White   
			new Vector3(0f, 0f, 0f)	 // Black     
		};
	}
}
