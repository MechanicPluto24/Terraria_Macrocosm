using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class MicronovaBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        private const float MaxBeamLength = 3000f;
        private const float BeamHitboxCollisionWidth = 22f;

        public int AITimer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        float Transparency = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 22;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (AITimer < 1)
            {
                Vector2 target = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                Projectile.velocity = target;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            /*
            Vector2 beamDims = new Vector2(Projectile.velocity.Length() * MaxBeamLength, Projectile.width * Projectile.scale);

            for (int i = 0; i < 2000; i++)
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * MaxBeamLength, i / 2000f);
                var d = Dust.NewDustPerfect(pos, DustID.Astra, Projectile.velocity.RotatedBy(i % 2 == 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2) * Main.rand.NextFloat(0.2f, 0.5f));
                d.alpha = 127; 
                d.noGravity = true; 
            }
            */

            AITimer++;
            if (AITimer < 5)
                Transparency += 0.25f;
            if (AITimer > 25)
                Transparency -= 0.25f;
        }

        public override bool? CanHitNPC(NPC npc)
        {
            return !npc.friendly;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
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
            centerFloored += new Vector2(0, Main.rand.NextFloat(-8f, 3f)).RotatedBy(Projectile.velocity.ToRotation());

            Vector2 drawScale = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            float visualBeamLength = MaxBeamLength - 14.5f * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 startPosition = centerFloored - Main.screenPosition;
            Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;

            // Draw the outer beam.

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            DrawBeam(Main.spriteBatch, texture, startPosition, endPosition, drawScale, new Color(171, 255, 255) * Transparency * (0.9f + (0.1f * ((float)(Math.Sin(AITimer))))));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            // Draw the inner beam, which is half size.

            // Returning false prevents Terraria from trying to draw the Projectile itself.
            return false;
        }

        private void DrawBeam(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color beamColor)
        {
            Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
            DelegateMethods.c_1 = beamColor;
            Utils.DrawLaser(spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
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
            Vector2 beamEndPos = Projectile.Center + Projectile.velocity * MaxBeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }
    }
}