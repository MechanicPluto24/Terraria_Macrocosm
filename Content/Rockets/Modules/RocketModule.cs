using Macrocosm.Common.Customization;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract partial class RocketModule : ModType, ILocalizedModType, IPatternable
    {
        public string LocalizationCategory => "UI.Rocket.Modules";
        //public LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
        public LocalizedText DisplayName => Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Modules." + Name + ".DisplayName", PrettyPrintName);

        protected sealed override void Register()
        {
        }

        public bool Active { get; set; }

        public Vector2 Position { get; set; }
        public virtual Vector2 Offset => Vector2.Zero;

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

        public abstract AssemblyRecipe Recipe { get; }

        public Detail Detail { get; set; }

        public Pattern Pattern { get; set; }
        public string PatternContext => Name;

        public bool HasPattern => Pattern != default;
        public bool HasDetail => Detail != default;
        private bool SpecialDraw => HasPattern || HasDetail;

        /// <summary> This module's draw priority </summary>
        public abstract int DrawPriority { get; }

        /// <summary> The module's draw origin </summary>
        protected virtual Vector2 Origin => new(0, 0);

        public bool IsBlueprint { get; set; } = false;

        public virtual bool Interactible => true;

        public virtual string TexturePath => Utility.GetNamespacePath(this);
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad).Value;

        public virtual string IconPath => (GetType().Namespace + "/Icons/" + Name).Replace(".", "/");
        public Texture2D Icon => ModContent.Request<Texture2D>(IconPath, AssetRequestMode.ImmediateLoad).Value;

        public virtual string BlueprintPath => (GetType().Namespace + "/Blueprints/" + Name).Replace(".", "/");
        public Texture2D Blueprint => ModContent.Request<Texture2D>(BlueprintPath, AssetRequestMode.ImmediateLoad).Value;
        public bool BlueprintHighlighted { get; set; } = false;

        public Color BlueprintOutlineColor = UITheme.Current.PanelStyle.BorderColor;
        public Color BlueprintFillColor = UITheme.Current.PanelStyle.BackgroundColor;

        protected Rocket rocket;

        public RocketModule()
        {
            Detail = default;
            Pattern = PatternManager.Get("Basic", Name);
        }

        public void SetRocket(Rocket value) => rocket = value;

        public virtual void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
        }

        private static Asset<Effect> colorMaskShading;
        private SpriteBatchState state;
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // Load current pattern and apply shader 
            state.SaveState(spriteBatch);
            SamplerState samplerState1 = Main.graphics.GraphicsDevice.SamplerStates[12];
            SamplerState samplerState2 = Main.graphics.GraphicsDevice.SamplerStates[2];
            if (SpecialDraw)
            {
                Effect effect = null;
                if (HasPattern)
                {
                    effect = Pattern.GetEffect();
                    Main.graphics.GraphicsDevice.Textures[1] = Pattern.Texture.Value;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
                }

                if (HasDetail)
                {
                    // Pass the detail to the shader via the S2 register
                    Main.graphics.GraphicsDevice.Textures[2] = Detail.Texture.Value;
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

                // Clear the tex register  
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

            colorMaskShading ??= ModContent.Request<Effect>(Macrocosm.ShadersPath + "ColorMaskShading", AssetRequestMode.ImmediateLoad);
            Effect effect = colorMaskShading.Value;

            Main.graphics.GraphicsDevice.Textures[1] = Blueprint;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            effect.Parameters["uColorCount"].SetValue(2);
            effect.Parameters["uColorKey"].SetValue(blueprintKeys);
            effect.Parameters["uColor"].SetValue((new Color[]
            {
                BlueprintHighlighted ? UITheme.Current.ButtonHighlightStyle.BorderColor : UITheme.Current.PanelStyle.BorderColor,
                UITheme.Current.PanelStyle.BackgroundColor
            }).ToVector4Array());
            effect.Parameters["uSampleBrightness"].SetValue(false);

            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

            spriteBatch.Draw(Blueprint, Position + position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);

            Main.graphics.GraphicsDevice.Textures[1] = null;
            Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
        }

        protected readonly Vector3[] blueprintKeys = [
            new Vector3(0.47f, 0.47f, 0.47f),
            new Vector3(0.74f, 0.74f, 0.74f)
        ];
    }
}
