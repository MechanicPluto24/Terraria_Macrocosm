using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles;

public class LunarRustStar : Particle
{
    public override string Texture => Macrocosm.EmptyTexPath;
    public float Opacity { get; set; }
    public int StarPointCount { get; set; } = 1;
    private int rotationDirection = 1;

    public override void SetDefaults()
    {
        ScaleVelocity = new(-0.045f);
        Opacity = 0.8f;
        Color = Main.rand.Next(2) switch
        {
            0 => new(188, 89, 134),
            _ => new(33, 188, 190),
        };
        StarPointCount = Main.rand.Next(1, 4);
    }

    public override void OnSpawn()
    {
        rotationDirection = Main.rand.NextDirection();
    }

    public override void AI()
    {
        Lighting.AddLight(Center, Color.ToVector3());

        Opacity -= 0.01f;
        Rotation += 0.05f * rotationDirection;

        if (Scale.X < 0.002f)
            Kill();
    }

    public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        Utility.DrawStar(Position - screenPosition, StarPointCount, Color * Opacity, Scale, Rotation);
        return false;
    }
}
