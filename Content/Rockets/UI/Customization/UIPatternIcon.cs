using Macrocosm.Common.Customization;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Rockets.UI.Customization;

public class UIPatternIcon : UIPanelIconButton, IFocusable
{
    public Pattern Pattern { get; }
    private readonly Asset<Texture2D> panel;
    public UIPatternIcon(Pattern pattern)
    : base
    (
        Macrocosm.EmptyTex,
        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
    )
    {
        Pattern = PatternManager.Get(pattern.Name, "Icon");
        foreach (var colorData in pattern.ColorData)
            Pattern.ColorData[colorData.Key] = colorData.Value;

        panel = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad);
    }

    public override void OnInitialize()
    {
        FocusContext = "PatternSelection";
        OnLeftClick += (_, _) => { HasFocus = true; };
        HoverText = Language.GetOrRegister("Mods.Macrocosm.UI.Rocket.Customization.Patterns." + Pattern.Name, () => Pattern.Name);
    }

    private SpriteBatchState state;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var dimensions = GetOuterDimensions();

        Effect effect = Pattern.PrepareEffect();
        SamplerState samplerState = spriteBatch.GraphicsDevice.SamplerStates[1];
        Main.graphics.GraphicsDevice.Textures[1] = Pattern.Texture.Value;
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
