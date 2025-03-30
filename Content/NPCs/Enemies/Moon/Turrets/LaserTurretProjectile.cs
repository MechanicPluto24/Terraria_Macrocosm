using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon.Turrets
{
    public class LaserTurretProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        private const float MaxBeamLength = 3000f;
        private const float BeamHitboxCollisionWidth = 22f;

        public int AITimer=0;
        


        float Transparency = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 22;
            Projectile.hide = true;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 35;
   
        }
        public override void OnKill (int timeLeft){
            NPC OwnerTurret= Main.npc[(int)Projectile.ai[0]];
            if(OwnerTurret.active && OwnerTurret.type == ModContent.NPCType<LaserTurret>())
                OwnerTurret.ai[0]=0f;
        }
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {            
            NPC OwnerTurret= Main.npc[(int)Projectile.ai[0]];
            if(!OwnerTurret.active || OwnerTurret.type != ModContent.NPCType<LaserTurret>())
                Projectile.Kill();

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.rotation = Projectile.velocity.ToRotation();


            AITimer++;

            if (Transparency < 1f && AITimer < 25)
                Transparency += 0.1f;
            else
                Transparency -= 0.2f;
        }

     

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
            => Color.White * (1f - Projectile.alpha / 255f);


        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined direction, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;

            Vector2 drawScale = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            float visualBeamLength = MaxBeamLength - 14.5f * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 startPosition = centerFloored - Main.screenPosition;
            Vector2 endPosition = startPosition + Projectile.velocity * Utility.CastLength(startPosition, new Vector2(1, 0).RotatedBy(Projectile.rotation), 2000f, false);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Color color = new Color(255, 255, 255).WithAlpha(225) * Transparency * (0.9f + (0.1f * MathF.Sin(AITimer)));
            
            Vector2 visualStartPosition = startPosition;
            Utility.DrawBeam(texture, visualStartPosition, endPosition, drawScale, color, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity * Utility.CastLength(Projectile.Center, new Vector2(1, 0).RotatedBy(Projectile.rotation), 2000f, false);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }
    }
}