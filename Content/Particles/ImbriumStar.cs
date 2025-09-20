using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles;

public class ImbriumStar : Particle
{
    public override string Texture => Macrocosm.EmptyTexPath;
    public float Opacity { get; set; }

    public override void SetDefaults()
    {
        ScaleVelocity = new(-0.035f);
        Opacity = 0.8f;
        Color = Color.Lerp(Color.White, new Color(0, 217, 102, 255), 0.1f + 0.7f * Main.rand.NextFloat());
    }

    public override void OnSpawn()
    {
    }

    public override void AI()
    {
        if (Scale.X < 0.002f)
            Kill();
    }

    public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        Utility.DrawStar(Position - screenPosition, 2, Color.WithOpacity(Opacity), Scale);
        return false;
    }
}
