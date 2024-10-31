using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ManisolBladeMoon : ManisolBladeBase
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            maxPenetrateCount = 5;
        }

        public override void OnStick()
        {
        }

        public override void OnRecalled()
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            if (AI_State == ActionState.Returning && target.whoAmI != npcStick)
            {
                target.AddBuff(BuffID.BrokenArmor, 300, false);

                Particle.Create<PrettySparkle>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = Projectile.velocity * 0.1f;
                    p.Color = new Color(113, 150, 150, 255);
                    p.Scale = new Vector2(4.5f, 1.2f) * (Projectile.velocity.LengthSquared() / (returnSpeed * returnSpeed));
                    p.Rotation = Projectile.velocity.ToRotation();
                    p.TimeToLive = 25;
                    p.DrawVerticalAxis = false;
                });
            }
        }

        public override void AI()
        {
            if (!Main.dedServ && AI_State != ActionState.Stick)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 position = Projectile.Center + Projectile.velocity + (Main.rand.NextVector2Circular(10, 10) * MathHelper.Clamp(DropTimer / maxDropTime, 0f, 1f));
                    Vector2 velocity = new Vector2(-Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f).RotatedByRandom(MathHelper.Pi / 32f) * Main.rand.NextFloat(0.5f, 3.5f);

                    if (DropTimer <= maxDropTime || AI_State == ActionState.Returning)
                        position = Projectile.Center;

                    Dust dust = Dust.NewDustPerfect(position, DustID.SilverFlame, velocity, Scale: 1.2f);
                    dust.color = new Color(113, 150, 150, 0);
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int length = 0;
            if (AI_State == ActionState.Thrown)
                length = Projectile.oldPos.Length / 2;
            else if (AI_State == ActionState.Returning)
                length = Projectile.oldPos.Length;

            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;

                Color trailColor = new Color(113, 150, 150, 0) * (((float)length - i) / length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.spriteBatch.Draw(TextureAssets.Extra[ExtrasID.SharpTears].Value, drawPos, null, trailColor, Projectile.oldRot[i] + MathHelper.PiOver2, TextureAssets.Extra[ExtrasID.SharpTears].Size() / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            /*
            if(AI_State == ActionState.Returning)
                Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.SharpTears].Value, Projectile.Center - Main.screenPosition, null, new Color(113, 150, 150).WithAlpha(0), Projectile.rotation, TextureAssets.Extra[ExtrasID.SharpTears].Size() / 2f, new Vector2(Projectile.scale * 0.8f, Projectile.scale * 1.84f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            */

            return base.PreDraw(ref lightColor);
        }
    }
}
