using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract partial class RocketModule
    {
        public string Name => GetType().Name;

        public string FullName => GetType().Namespace + "." + Name;

        /// <summary> This module's draw priority </summary>
        public abstract int DrawPriority { get; }

        public Detail Detail { get; set; }
        public Pattern Pattern { get; set; }

		public bool HasPattern => Pattern != default;
		public bool HasDetail => Detail is not null;
		private bool SpecialDraw => HasPattern || HasDetail;

        public Vector2 Position { get; set; }
        public Vector2 Center
        {
            get => Position + Size / 2f;
            set => Position = value - Size / 2f;
        }

        public abstract int Width { get; }
        public abstract int Height { get; }

        /// <summary> The module's hitbox size as a vector </summary>
        public Vector2 Size => new(Hitbox.Width, Hitbox.Height);

        /// <summary> The module's collision hitbox. </summary>
        public virtual Rectangle Hitbox => new((int)Position.X, (int)Position.Y, Width, Height);

        /// <summary> The module's draw origin </summary>
        protected virtual Vector2 Origin => new(0, 0);

        public virtual string TexturePath => Utility.GetNamespacePath(this);
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

        protected Rocket rocket;

        public RocketModule(Rocket rocket)
        {
            Pattern = CustomizationStorage.GetDefaultPattern(GetType().Name);
            this.rocket = rocket;
        }

        public virtual void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

        }

        private SpriteBatchState state;
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            // Load current pattern and apply shader 
            state.SaveState(spriteBatch);
            SamplerState samplerState1 = Main.graphics.GraphicsDevice.SamplerStates[1];
            SamplerState samplerState2 = Main.graphics.GraphicsDevice.SamplerStates[2];
            if (SpecialDraw)
            {
                // Load the coloring shader
                Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad).Value;

                if (HasPattern)
                {
                    // Pass the pattern to the shader via the S1 register
                    Main.graphics.GraphicsDevice.Textures[0] = Texture;
                    Main.graphics.GraphicsDevice.Textures[1] = Pattern.Texture;
                    Main.graphics.GraphicsDevice.Textures[2] = null;

                    // Change sampler state for proper alignment at all zoom levels 
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

                    //Pass the color mask keys as Vector3s and configured colors as Vector4s
                    //Note: parameters are scalars intentionally, I manually unrolled the loop in the shader to reduce number of branch instructions -- Feldy
                    for (int i = 0; i < Pattern.MaxColorCount; i++)
                    {
                        effect.Parameters["uColorKey" + i.ToString()].SetValue(Pattern.ColorKeys[i]);
                        effect.Parameters["uColor" + i.ToString()].SetValue(Pattern.GetColor(i).ToVector4());
                    }

                    // Get a blend between the general ambient color at the rocket center, and the local color on this module's center
                    Color localColor = Color.Lerp(Lighting.GetColor((int)(Center.X) / 16, (int)(Center.Y) / 16), ambientColor, 0.8f);

                    //Pass the ambient lighting on the rocket 
                    effect.Parameters["uAmbientColor"].SetValue(localColor.ToVector3());
                }

                if (HasDetail)
                {
                    // Pass the detail to the shader via the S2 register
                    Main.graphics.GraphicsDevice.Textures[2] = Detail.Texture;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

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
                Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState1;
                Main.graphics.GraphicsDevice.SamplerStates[2] = samplerState2;
            }
        }

        public virtual void DrawOverlay(SpriteBatch spriteBatch, Vector2 screenPos)
        {

        }
    }
}
