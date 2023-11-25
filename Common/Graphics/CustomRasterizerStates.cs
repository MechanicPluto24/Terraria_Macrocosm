using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Graphics
{
    public class CustomRasterizerStates
    {
        public static readonly RasterizerState ScissorTest = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };
    }
}
