using Macrocosm.Common.Customization;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public abstract partial class RocketModule : ModType, ILocalizedModType
    {
        public enum ConfigurationType
        {
            Any,
            Manned,
            Unmanned
        }

        /// <summary> List of all the existing rocket modules </summary>
        public static List<RocketModule> Templates { get; } = new();

        public static List<RocketModule> DefaultModules => Templates
                .Where(m => m.Configuration != ConfigurationType.Unmanned)
                .GroupBy(m => m.Slot)
                .Select(g => g.OrderBy(m => m.Tier).First().Clone()) 
                .OrderBy(m => m.Slot)
                .ToList();

        // For backwards compatibility
        public static List<RocketModule> DefaultLegacyModules => new List<RocketModule>()
        {
            Templates.FirstOrDefault(m => m.Name == "CommandPod"),
            Templates.FirstOrDefault(m => m.Name == "ServiceModule"),
            Templates.FirstOrDefault(m => m.Name == "ReactorModule"),
            Templates.FirstOrDefault(m => m.Name == "EngineModuleMk2"),
            Templates.FirstOrDefault(m => m.Name == "BoosterLeft"),
            Templates.FirstOrDefault(m => m.Name == "BoosterRight")
        }.Where(m => m != null).ToList();

        protected sealed override void Register()
        {
            ModTypeLookup<RocketModule>.Register(this);
            Templates.Add(this);
        }
        public override void SetupContent() => SetStaticDefaults();
        public override void SetStaticDefaults()
        {
            foreach(AssemblyRecipeEntry entry in Recipe)
            {
                if (entry.ItemType.HasValue)
                    ItemID.Sets.IsAMaterial[entry.ItemType.Value] = true;
            }
        }

        public abstract int Slot { get; }
        public abstract int Tier { get; }
        public abstract ConfigurationType Configuration { get; }
        public abstract AssemblyRecipe Recipe { get; }

        public string LocalizationCategory => "UI.Rocket.Modules";
        public LocalizedText DisplayName => Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Modules." + Name + ".DisplayName", PrettyPrintName);

        public Vector2 Position { get; set; }
        public virtual Vector2 Offset => Vector2.Zero;

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);


        public Pattern Pattern { get; set; }
        public Decal Decal { get; set; }

        public bool HasPattern => Pattern != default;
        public bool HasDecal => Decal != default;
        private bool SpecialDraw => HasPattern || HasDecal;

        /// <summary> This module's draw priority </summary>
        public abstract int DrawPriority { get; }

        /// <summary> The module's draw origin </summary>
        protected virtual Vector2 Origin => new(0, 0);

        public bool IsBlueprint { get; set; } = false;

        public virtual bool Interactible => true;

        public virtual string TexturePath => Utility.GetNamespacePath(this);

        private Asset<Texture2D> texture;
        public Texture2D Texture => (texture ??= ModContent.Request<Texture2D>(TexturePath, AssetRequestMode.ImmediateLoad)).Value;

        public virtual string IconPath => (GetType().Namespace + "_Icon").Replace(".", "/");
        private Asset<Texture2D> icon;
        public Texture2D Icon => (icon ??= ModContent.RequestIfExists<Texture2D>(IconPath, out var asset, AssetRequestMode.ImmediateLoad) ? asset : Macrocosm.EmptyTex).Value;

        public virtual string BlueprintPath => (GetType().Namespace + "Blueprint").Replace(".", "/");
        private Asset<Texture2D> blueprint;
        public Texture2D Blueprint => (blueprint ??= ModContent.RequestIfExists<Texture2D>(BlueprintPath, out var asset, AssetRequestMode.ImmediateLoad) ? asset : Macrocosm.EmptyTex).Value;


        public bool BlueprintHighlighted { get; set; } = false;

        public Color BlueprintOutlineColor = UITheme.Current.PanelStyle.BorderColor;
        public Color BlueprintFillColor = UITheme.Current.PanelStyle.BackgroundColor;

        protected Rocket rocket;

        public RocketModule()
        {
            Decal = default;
            Pattern = PatternManager.Get("Basic", Name);
        }

        public RocketModule Clone() => DeserializeData(SerializeData());
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
            state.SaveState(spriteBatch);
            if (SpecialDraw)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, null, state.Matrix);
                Pattern?.Apply();
                //TODO: Decal?.Pattern.Apply();
            }

            spriteBatch.Draw(Texture, position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

            if (SpecialDraw)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
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
