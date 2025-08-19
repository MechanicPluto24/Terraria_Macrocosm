using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Customization;

public class UIModuleIcon : UIPanel
{
    private RocketModule module;
    public void SetModule(RocketModule module)
    {
        this.module = module;
    }

    public void ClearModule()
    {
        module = null;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (module is null)
            return;

        Recalculate();
        CalculatedStyle dimensions = GetDimensions();
        Vector2 iconPosition = dimensions.Center();
        Texture2D icon = module.Icon;
        spriteBatch.Draw(icon, iconPosition, null, Color.White, 0f, new Vector2(icon.Width * 0.5f, icon.Height * 0.5f), GetIconScale(module.Name), SpriteEffects.None, 0);
    }

    private float GetIconScale(string moduleName)
    {
        return moduleName switch
        {
            "ServiceModule" or "UnmannedTug" => 0.9f,
            "ReactorModule" or "StrucureModule" => 1.15f,
            "CommandPod" => 1.25f,
            "PayloadPod" => 1f,
            "EngineModuleMk1" or "EngineModuleMk2" => 0.34f,
            "BoosterLeft" or "BoosterRight" => 0.34f,
            "LandingLegLeft" or "LandingLegRight" => 0.34f,
            _ => 1f,
        };
    }
}
