using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Rockets.UI.Customization
{
    public class UIPatternIcon : UIPanelIconButton, IFocusable
    {
        public Pattern Pattern { get; set; }
        private Asset<Texture2D> panel;
        public UIPatternIcon(Pattern pattern)
        : base
        (
            Macrocosm.EmptyTex,
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
        )
        {
            Pattern = pattern;
            panel = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad);
        }

        public override void OnInitialize()
        {
            FocusContext = "PatternSelection";
            OnLeftClick += (_, _) => { HasFocus = true; };
            HoverText = Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Customization.Patterns." + Pattern.Name, () => Pattern.Name);
        }

        private static Asset<Effect> colorMaskShading;
        private SpriteBatchState state;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var dimensions = GetOuterDimensions();

            Effect effect = Pattern.GetEffect();
            SamplerState samplerState = spriteBatch.GraphicsDevice.SamplerStates[1];

            Main.graphics.GraphicsDevice.Textures[1] = Pattern.Icon.Value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, state.BlendState, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

            spriteBatch.Draw(panel.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 0.995f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(state);
            Main.graphics.GraphicsDevice.Textures[1] = null;
            Main.graphics.GraphicsDevice.SamplerStates[1] = samplerState;
        }
    }
}
