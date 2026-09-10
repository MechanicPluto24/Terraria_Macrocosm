using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;

namespace Macrocosm.Common.Bases.Projectiles;

/// <summary>
/// Base class for a bullet projectile that bounces from enemy to enemy. 
/// </summary>
public abstract class RailgunProjectile : ModProjectile
{
    
    public override string Texture => "Macrocosm/Common/Bases/Projectiles/RailgunProjectile";
    public virtual Color beamColor => Color.White;
    public virtual int dustType => -1;
    public virtual float visualScale=> 1f;
    public virtual void HitTileBehaviour(){}

    public Vector2 beamStart = new Vector2();
    public Vector2 beamEnd = new Vector2();
    private bool killed = false;
    private int stickTime;
    private bool hitTile=false;
    
    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 20;
        Projectile.friendly = true;

        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;

        Projectile.extraUpdates = 0;
        Projectile.tileCollide = false;
        stickTime = 60;
        beamStart = Projectile.Center;
    }

    public override bool ShouldUpdatePosition() => false;
    
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (Projectile.timeLeft < 18)
            return false;


        // Otherwise, perform an AABB line collision check to check the whole beam.
        float _ = float.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamStart, beamEnd, 4 * Projectile.scale, ref _);
    }



    public override void AI()
    {
        if (!killed)
        {
            beamStart = Projectile.Center;
            Vector2 aim = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            beamEnd = beamStart + aim * Utility.CastLength(beamStart, aim, 3000f, false);
            if((beamEnd-beamStart).Length() < 3000f)
                hitTile=true;
            killed=true;
            if(dustType != -1 && hitTile == true)
            {
                for (int i=0; i < Main.rand.Next(15,25); i++)
                {
                    Vector2 vector = (-Projectile.velocity.SafeNormalize(Vector2.UnitX)).RotatedBy(Main.rand.NextFloat(-0.3f,0.3f))*Main.rand.NextFloat(4f,15f);
                    Dust d = Dust.NewDustDirect(beamEnd, 0, 0, dustType, vector.X, vector.Y, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.noGravity = true;
                    d.noLight=false;
                }
            }
            if(hitTile == true)
            {
                HitTileBehaviour();
            }

        }
        Projectile.Opacity-=(1f/20f);
        Lighting.AddLight(beamEnd, (beamColor* Projectile.Opacity).ToVector3() * 0.4f);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => false;

    private void FalseKill()
    {
    }

    public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        state.SaveState(Main.spriteBatch);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        Asset<Texture2D> texture = TextureAssets.Projectile[Type];
        Color color = beamColor;
        Asset<Texture2D> flashTexture = ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Star7");


        Vector2 beamOrigin = (texture.Size() / 2f);
        float beamSegmentLength = texture.Height();
        Vector2 beamDrawPosition = beamStart;
        float beamLengthRemainingToDraw = (beamEnd - beamStart).Length() + beamSegmentLength / 2f;
        Vector2 unitVectorToEnd = (beamEnd - beamStart).SafeNormalize(Vector2.Zero);
        float fadeIn=0f;

        while (beamLengthRemainingToDraw > 0f)
        {
            if(fadeIn<1f)
                fadeIn+=0.1f;
            Main.spriteBatch.Draw(texture.Value, beamDrawPosition - Main.screenPosition, null, beamColor.WithOpacity(Projectile.Opacity*fadeIn), Projectile.velocity.ToRotation()+MathHelper.PiOver2, beamOrigin, new Vector2(visualScale,1f), SpriteEffects.None, 0f);

            beamDrawPosition += unitVectorToEnd * beamSegmentLength;
            beamLengthRemainingToDraw -= beamSegmentLength;
        }
        if(hitTile == true)
            Main.spriteBatch.Draw(flashTexture.Value, beamEnd - Main.screenPosition, null, beamColor.WithOpacity(1f), Projectile.velocity.ToRotation()+MathHelper.PiOver2, flashTexture.Size() /2f, Projectile.Opacity*Projectile.Opacity*0.4f*visualScale, SpriteEffects.None, 0f);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);
        return false;
    }

}
