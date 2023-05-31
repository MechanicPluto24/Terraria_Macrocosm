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
		public Texture2D PaintMask
		{
			get {
				if (ModContent.RequestIfExists(TexturePath + "_PaintMask", out Asset<Texture2D> paintMask, AssetRequestMode.ImmediateLoad))
					return paintMask.Value;
				else
					return null;
			}
		}
		public Vector2 Position { get; set; }

		public bool HasPaintMask => PaintMask is not null;

		public Color AccentColor, LightHullColor, DarkHullColor, GlassColor = new(255, 255, 255, 0);

		public virtual void Draw(SpriteBatch spriteBatch, Color ambientColor)
		{
			AccentColor = new Color(3, 220, 102).NewAlpha(1f);
			LightHullColor = new Color(30, 250, 106).NewAlpha(0.4f);
			DarkHullColor = new Color(31, 120, 87).NewAlpha(0.8f);
			GlassColor = Color.Blue.NewAlpha(1f);

			Color[] colors = { AccentColor, LightHullColor, DarkHullColor, GlassColor };

			SpriteBatchState state = spriteBatch.SaveState(); 
			
			if (HasPaintMask)
			{
				Effect effect = ModContent.Request<Effect>("Macrocosm/Assets/Effects/ColorMaskShading",AssetRequestMode.ImmediateLoad).Value;

				Main.graphics.GraphicsDevice.Textures[1] = PaintMask;
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

				int numberOfColorKeys = effect.Parameters["COLOR_MAP_SIZE"].GetValueInt32();

				for(int i = 0; i < numberOfColorKeys; i++)
				{
					effect.Parameters["uColor" + (i+1).ToString() + "Key"].SetValue(ColorKeys[i].ToVector3());
					effect.Parameters["uColor" + (i+1).ToString()].SetValue(colors[i].ToVector4());
				}

				effect.Parameters["uAmbientColor"].SetValue(ambientColor.ToVector3());

				spriteBatch.End();
				spriteBatch.Begin(effect, state);
			}

			Vector2 origin = new(Texture.Width / 2, 0);
			spriteBatch.Draw(Texture, Position, null, ambientColor, 0f, origin, 1f, SpriteEffects.None, 0f);

			if (HasPaintMask)
			{
				spriteBatch.End();
				spriteBatch.Restore(state);
			}
		}

		/// <summary> 
		/// Color mask keys
		/// 1. Cyan - Accent color (for example, the tip)
		/// 2. Magenta - Light hull color
		/// 3. Yellow - Dark hull color
		/// 4. Red - Glass windows color
		/// </summary>
		public static readonly Color[] ColorKeys = {
			new Color(0, 255, 255),
			new Color(255, 0, 255),
			new Color(255, 255, 0),
			new Color(255, 0, 0)
		};
	}
}
