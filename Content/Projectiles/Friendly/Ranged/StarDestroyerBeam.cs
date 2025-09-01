using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class StarDestroyerBeam : ModProjectile
{
    public enum BeamVariant { Blue, Yellow }
    public BeamVariant BeamType
    {
        get => (Projectile.ai[0] == 0f ? BeamVariant.Blue : BeamVariant.Yellow);
        set => Projectile.ai[0] = (float)value;
    }

    private bool spawned;
    private Color color = default;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
    }

    public override string Texture => Macrocosm.EmptyTexPath;
    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 4;
        Projectile.timeLeft = 600;
        Projectile.scale = 0.3f;
        Projectile.alpha = 255;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        if (!spawned)
        {
            if (BeamType is BeamVariant.Blue)
                color = new Color(100, 100, 255, 0);
            else if (BeamType is BeamVariant.Yellow)
                color = new Color(255, 180, 25, 0);

            spawned = true;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

        if (Projectile.alpha > 0)
            Projectile.alpha -= 15;
    }

    public override void OnKill(int timeLeft)
    {
        for (float i = 0f; i < 1f; i += 0.33f)
            Dust.NewDustPerfect(Projectile.Center, 278, Projectile.oldVelocity.RotatedByRandom(0.2) * Main.rand.NextFloat(0.3f), 0, color * 0.5f).noGravity = true;
    }

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
        Rectangle sourceRect = tex.Frame(1, Main.projFrames[base.Type], frameY: Projectile.frame);
        Vector2 origin = Projectile.Size / 2f;
        state.SaveState(Main.spriteBatch);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);

        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, (color) * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, new Vector2(Projectile.scale * 2f, Projectile.scale * 8f), SpriteEffects.None, 0);
        return false;
    }
}

