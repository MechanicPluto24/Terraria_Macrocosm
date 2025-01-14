using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ManisolBladeSol : ManisolBladeBase
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (!Main.dedServ && AI_State != ActionState.Stick)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 position = Projectile.Center + Projectile.velocity + (Main.rand.NextVector2Circular(10, 10) * MathHelper.Clamp(DropTimer / maxDropTime, 0f, 1f));
                    Vector2 velocity = new Vector2(-Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f).RotatedByRandom(MathHelper.Pi / 32f) * Main.rand.NextFloat(0.5f, 3.5f);

                    if (DropTimer <= maxDropTime || AI_State == ActionState.Returning)
                        position = Projectile.Center;

                    Dust dust = Dust.NewDustPerfect(position, DustID.OrangeTorch, velocity, Scale: 1.2f);
                    dust.noGravity = true;
                }
            }
        }

        public override void OnRecalled()
        {
            float strength = npcStick > 0 ? 1.5f : 1f;
            Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ManisolBladeSolExplosion>(), Projectile.damage, 12f, Main.myPlayer, ai0: strength);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Melting>(), 270, false);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int length = 0;
            if (AI_State == ActionState.Thrown)
                length = Projectile.oldPos.Length / 2;
            else if (AI_State == ActionState.Returning)
                length = Projectile.oldPos.Length / 2;

            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color trailColor = new Color(165, 146, 90, 0) * (((float)length - i) / length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.SharpTears].Value, drawPos, null, trailColor, Projectile.oldRot[i] + MathHelper.PiOver2, TextureAssets.Extra[ExtrasID.SharpTears].Size() / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
