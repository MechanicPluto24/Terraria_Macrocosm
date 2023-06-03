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

		public string PatternName = "Basic";
		public string DetailName = "FlagRO";

		public Texture2D PatternTexture
		{
			get {
				if (ModContent.RequestIfExists(TexturePath + "_Pattern_" + PatternName, out Asset<Texture2D> paintMask))
					return paintMask.Value;
				else
					return Macrocosm.EmptyTex;
			}
		}

		public Texture2D DetailTexture
		{
			get
			{
				if (ModContent.RequestIfExists(TexturePath + "_Detail_" + DetailName, out Asset<Texture2D> paintMask))
					return paintMask.Value;
				else
					return Macrocosm.EmptyTex;
			}
		}

		public Vector2 Position { get; set; }
		protected Vector2 Origin => new(Texture.Width / 2, 0);

		public bool HasPattern => PatternTexture is not null;
		public bool HasDetail => DetailTexture is not null;

		private bool SpecialDraw => HasPattern || HasDetail;

		public Color[] Colors = new Color[8];
 

		public virtual void Draw(SpriteBatch spriteBatch, Color ambientColor)
		{
			Array.Fill(Colors, Color.White.NewAlpha(0f));

			// Load current pattern and apply shader 
			SpriteBatchState state = spriteBatch.SaveState(); 
			if (SpecialDraw)
			{
				// -- testing, will be configured from an UI		
				if (this is EngineModule)
				{
					PatternName = "Binary";  		  
					Colors[0] = Color.White;
					Colors[1] = Color.White;
					Colors[2] = Color.Black;
					Colors[3] = Color.White;
					Colors[4] = Color.White;
					Colors[5] = Color.White;
					Colors[6] = Color.White;
					Colors[7] = Color.White;

					if (HasDetail)
						Main.graphics.GraphicsDevice.Textures[2] = DetailTexture;
				}

				// Load shader
				Effect effect = ModContent.Request<Effect>("Macrocosm/Assets/Effects/ColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

				// Pass the pattern to the shader via the S1 register
				Main.graphics.GraphicsDevice.Textures[1] = PatternTexture;

				// Configure the SamplerState of the pattern.
				// Needed for proper alignment of texture and coords at some zoom levels
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

				//Pass the color mask keys as Vector3s (parameters are scalars intentionally)
				effect.Parameters["uColorKey0"].SetValue(ColorKeys[0]);
				effect.Parameters["uColorKey1"].SetValue(ColorKeys[1]);
				effect.Parameters["uColorKey2"].SetValue(ColorKeys[2]);
				effect.Parameters["uColorKey3"].SetValue(ColorKeys[3]);
				effect.Parameters["uColorKey4"].SetValue(ColorKeys[4]);
				effect.Parameters["uColorKey5"].SetValue(ColorKeys[5]);
				effect.Parameters["uColorKey6"].SetValue(ColorKeys[6]);
				effect.Parameters["uColorKey7"].SetValue(ColorKeys[7]);

				// Pass the configured colors as Vector4s
				effect.Parameters["uColor0"].SetValue(Colors[0].ToVector4());
				effect.Parameters["uColor1"].SetValue(Colors[1].ToVector4());
				effect.Parameters["uColor2"].SetValue(Colors[2].ToVector4());
				effect.Parameters["uColor3"].SetValue(Colors[3].ToVector4());
				effect.Parameters["uColor4"].SetValue(Colors[4].ToVector4());
				effect.Parameters["uColor5"].SetValue(Colors[5].ToVector4());
				effect.Parameters["uColor6"].SetValue(Colors[6].ToVector4());
				effect.Parameters["uColor7"].SetValue(Colors[7].ToVector4());

				//Pass the ambient lighting on the rocket 
				effect.Parameters["uAmbientBrightness"].SetValue(ambientColor.GetLuminance());

				spriteBatch.End();
				spriteBatch.Begin(effect, state);
			}

			spriteBatch.Draw(Texture, Position, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			if (SpecialDraw)
			{
				spriteBatch.End();
				spriteBatch.Restore(state);
			}
		}

		/// <summary> Color mask keys </summary>
		public static readonly Vector3[] ColorKeys = {
			new Vector3(0f, 1f, 1f),     // Cyan    
			new Vector3(1f, 0f, 1f),     // Magenta 
			new Vector3(1f, 1f, 0f),     // Yellow  
			new Vector3(0f, 1f, 0f),     // Green   
			new Vector3(1f, 0f, 0f),     // Red
			new Vector3(0f, 0f, 1f),     // Blue   
			new Vector3(1f,.5f, 0f),     // Orange
			new Vector3(0f,.5f, 1f)      // Azure
		};

	}
}
