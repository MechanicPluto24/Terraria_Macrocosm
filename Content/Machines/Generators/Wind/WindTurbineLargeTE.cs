using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Wind;

public class WindTurbineLargeTE : WindTurbineTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineLarge>();
    protected override float BaseGeneratedPower => 120f;
    protected override int WindCheckHeight => 3;

    public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
    {
        basePosition.X += 4;
        basePosition.Y -= 52;
        base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
    }
}
