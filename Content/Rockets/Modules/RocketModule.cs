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
        public enum SlotType
        {
            Top,
            Service,
            Utilitary,
            Engine,
            LeftSide,
            RightSide
        }

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
                .GroupBy(m => (int)m.Slot)
                .Select(g => g.OrderBy(m => m.Tier).First().Clone()) 
                .OrderBy(m => (int)m.Slot)
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

        public Rocket Rocket { get; set; }

        public abstract SlotType Slot { get; }
        public abstract int Tier { get; }
        public abstract ConfigurationType Configuration { get; }
        public abstract AssemblyRecipe Recipe { get; }

        public string LocalizationCategory => "UI.Rocket.Modules";
        public LocalizedText DisplayName => Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Modules." + Name + ".DisplayName", PrettyPrintName);

        public Vector2 Position { get; set; }

        public virtual Vector2 GetOffset(RocketModule[] modules) => Vector2.Zero;

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

        public virtual string IconPath => (this.GetNamespacePath() + "_Icon").Replace(".", "/");
        private Asset<Texture2D> icon;
        public Texture2D Icon => (icon ??= ModContent.RequestIfExists<Texture2D>(IconPath, out var asset, AssetRequestMode.ImmediateLoad) ? asset : Macrocosm.EmptyTex).Value;

        public virtual string BlueprintPath => (this.GetNamespacePath() + "_Blueprint").Replace(".", "/");
        private Asset<Texture2D> blueprint;
        public Texture2D Blueprint => (blueprint ??= ModContent.RequestIfExists<Texture2D>(BlueprintPath, out var asset, AssetRequestMode.ImmediateLoad) ? asset : Macrocosm.EmptyTex).Value;

        public bool BlueprintHighlighted { get; set; } = false;

        public Color BlueprintOutlineColor = UITheme.Current.PanelStyle.BorderColor;
        public Color BlueprintFillColor = UITheme.Current.PanelStyle.BackgroundColor;

        public RocketModule()
        {
            Decal = default;
            Pattern = PatternManager.Get("Basic", Name);
        }

        public RocketModule Clone() => DeserializeData(SerializeData());

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
            Pattern blueprint = new("Blueprint", Name, BlueprintPath, new Dictionary<Color, PatternColorData>
            {
                { new Color(188, 188, 188), new PatternColorData( BlueprintHighlighted ? UITheme.Current.ButtonHighlightStyle.BorderColor : UITheme.Current.PanelStyle.BorderColor) },
                { new Color(119, 119, 119), new PatternColorData( UITheme.Current.PanelStyle.BackgroundColor ) }
            });

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, null, state.Matrix);

            blueprint.Apply();
            spriteBatch.Draw(Blueprint, Position + position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
