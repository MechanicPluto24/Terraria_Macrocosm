using Macrocosm.Common.Drawing.Trails;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace Macrocosm.Content.Trails;

public class ImbriumMeteorTrail : VertexTrail
{
    public override MiscShaderData TrailShader => GameShaders.Misc["MagicMissile"];

    public override Color TrailColors(float progressOnStrip)
    {
        return new Color(0, 217, 102, 255) * (1f - progressOnStrip);
    }

    public override float TrailWidths(float progressOnStrip)
    {
        return MathHelper.Lerp(36f, 32f, progressOnStrip);
    }
}
