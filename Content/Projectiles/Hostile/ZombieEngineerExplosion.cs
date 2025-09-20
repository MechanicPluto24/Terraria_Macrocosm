using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile;

public class ZombieEngineerExplosion : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;
    public ref float AI_Timer => ref Projectile.ai[0];

    private int defWidth;
    private int defHeight;
    private int sizeScale;
    public override void SetDefaults()
    {
        sizeScale = 4;
        defWidth = defHeight = 4;
        Projectile.width = defWidth;
        Projectile.height = defHeight;
        Projectile.hostile = true;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 25;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (AI_Timer == 0)
        {
            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = (new Color(25, 25, 25, 128));
                p.Scale = new(1.6f);
                p.NumberOfInnerReplicas = 1;
                p.ReplicaScalingFactor = 1f;
            });

            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = (new Color(255, 180, 100, 200));
                p.Scale = new(1.5f);
                p.NumberOfInnerReplicas = 8;
                p.ReplicaScalingFactor = 0.1f;
            });
        }

        AI_Timer++;
        Projectile.Resize((int)(defHeight * AI_Timer * sizeScale), (int)(defHeight * AI_Timer * sizeScale));
    }

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Flare3").Value;
        float progress = MathHelper.Clamp(AI_Timer / 25f, 0f, 1f);

        state.SaveState(Main.spriteBatch);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 100) * (1f - progress), 0f, texture.Size() / 2f, 1f - progress, SpriteEffects.None, 0f);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);

        return false;
    }
}
