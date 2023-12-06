using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
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

        public virtual string IconPath => (GetType().Namespace + "/Icons/" + Name).Replace(".", "/");
        public Texture2D Icon => ModContent.Request<Texture2D>(IconPath, AssetRequestMode.ImmediateLoad).Value;

        public virtual string BlueprintPath => (GetType().Namespace + "/Blueprints/" + Name).Replace(".", "/");
        public Texture2D Blueprint => ModContent.Request<Texture2D>(BlueprintPath, AssetRequestMode.ImmediateLoad).Value;


        protected Rocket rocket;

        public RocketModule(Rocket rocket)
        {
            Pattern = CustomizationStorage.GetDefaultPattern(GetType().Name);
            this.rocket = rocket;
        }

        public virtual void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position)
        {
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 position)
        {
        }

        private SpriteBatchState state;
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
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
                    Main.graphics.GraphicsDevice.Textures[1] = Pattern.Texture;
                    Main.graphics.GraphicsDevice.Textures[2] = null;

                    // Change sampler state for proper alignment at all zoom levels 
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

                    //Pass the color mask keys as Vector3s and configured colors as Vector4s
                    List<Vector4> colors = new();
                    for (int i = 0; i < Pattern.MaxColorCount; i++)
                         colors.Add(Pattern.GetColor(i).ToVector4());

                    effect.Parameters["uColorCount"].SetValue(Pattern.MaxColorCount);
                    effect.Parameters["uColorKey"].SetValue(Pattern.ColorKeys);
                    effect.Parameters["uColor"].SetValue(colors.ToArray());
                    effect.Parameters["uSampleBrightness"].SetValue(true);
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

            spriteBatch.Draw(Texture, position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

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

        public virtual void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
        {
            state.SaveState(spriteBatch);
            SamplerState samplerState = Main.graphics.GraphicsDevice.SamplerStates[1];
         
            Effect effect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad).Value;
            Main.graphics.GraphicsDevice.Textures[1] = Blueprint;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            effect.Parameters["uColorCount"].SetValue(2);
            effect.Parameters["uColorKey"].SetValue(blueprintKeys);
            effect.Parameters["uColor"].SetValue((new Color[] { UITheme.Current.PanelStyle.BorderColor, UITheme.Current.PanelStyle.BackgroundColor }).ToVector4Array());
            effect.Parameters["uSampleBrightness"].SetValue(false);

            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);
 
            spriteBatch.Draw(Blueprint, Position + position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);

            Main.graphics.GraphicsDevice.Textures[1] = null;
            Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
        }

        protected readonly Vector3[] blueprintKeys = new Vector3[] {
            new Vector3(0.47f, 0.47f, 0.47f),
            new Vector3(0.74f, 0.74f, 0.74f)
        };
    }
}
