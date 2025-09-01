using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macrocosm.Content.Rockets.Modules.Boosters;

public class BoosterLeft : BaseBooster
{
    public override SlotType Slot => SlotType.LeftSide;
    public override int Tier => 2;
    public override ConfigurationType Configuration => ConfigurationType.Any;

    public override int Width => 46;
    public override int Height => 314;
    public override Vector2 GlobalOffset => new(18, 0);

    public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
    {
        return new
        (
            x: 0,
            y: heights[0..4].Sum() - Height + 28
        );
    }

    public override Rectangle ModifyRenderBounds(Rectangle bounds, Rocket.DrawMode drawMode)
    {
        if (drawMode == Rocket.DrawMode.Dummy)
        {
            int extra = Width - (int)LandingLegDrawOffset.Value.X;
            return bounds with
            {
                Width = bounds.Width + extra,
                X = bounds.X + extra,
            };
        }

        return bounds;
    }

    public override Vector2? ExhaustOffset => new(14f, -28f);
    protected override Vector2? LandingLegDrawOffset => new(-78, 208);
    protected override int Direction => -1;


    public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk2>();

}
