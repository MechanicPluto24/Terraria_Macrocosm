using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Modules
{
	public abstract class RocketModule
	{
		public virtual string TexturePath => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

		public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

		public string PatternName = "Basic";
		public Texture2D Pattern
		{
			get {
				if (ModContent.RequestIfExists(TexturePath + "_Pattern_" + PatternName, out Asset<Texture2D> paintMask, AssetRequestMode.ImmediateLoad))
					return paintMask.Value;
				else
					return null;
			}
		}
		public Vector2 Position { get; set; }

		public bool HasPattern => Pattern is not null;

		public Color AccentColor, BackgroundColor, PrimaryColor, SecondaryColor = new(255, 255, 255, 0);

		public virtual void Draw(SpriteBatch spriteBatch, Color ambientColor)
		{
			AccentColor = Color.White;
			BackgroundColor = Color.White;
			PrimaryColor = new Color(40, 40, 40);
			SecondaryColor = Color.White;

			Color[] colors = { AccentColor, BackgroundColor, PrimaryColor, SecondaryColor };

			SpriteBatchState state = spriteBatch.SaveState(); 
			
			if (HasPattern)
			{
				Effect effect = ModContent.Request<Effect>("Macrocosm/Assets/Effects/ColorMaskShading",AssetRequestMode.ImmediateLoad).Value;

				PatternName = "Delta";

				Main.graphics.GraphicsDevice.Textures[1] = Pattern;
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

				//int numberOfColorKeys = effect.Parameters["COLOR_MAP_SIZE"].GetValueInt32();

				effect.Parameters["uColorKeys"].SetValue(ColorKeys);
				effect.Parameters["uColors"].SetValue(colors.ToVector4Array());
		 
				effect.Parameters["uAmbientColor"].SetValue(ambientColor.ToVector3());

				spriteBatch.End();
				spriteBatch.Begin(effect, state);
			}

			Vector2 origin = new(Texture.Width / 2, 0);
			spriteBatch.Draw(Texture, Position, null, ambientColor, 0f, origin, 1f, SpriteEffects.None, 0f);

			if (HasPattern)
			{
				spriteBatch.End();
				spriteBatch.Restore(state);
			}
		}

		/// <summary> 
		/// Color mask keys
		/// 1. Cyan - Accent color (for example, the tip)
		/// 2. Magenta - Background hull color
		/// 3. Yellow - Primary hull color
		/// 4. Red - Secondary hull color 
		/// </summary>
		public static readonly Vector3[] ColorKeys = {
			new Vector3(0f, 1f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 0f, 0f)
		};
	}
}
