using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ManisolBladeMoon : ManisolBladeBase
    {
        private int penerateCount;
        public override void SetDefaults()
        {
            base.SetDefaults();
            dustType = DustID.Shadowflame;
        }

        public override void OnReturn()
        {
            for(int i = 0; i < 5; i++)
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, new Vector2(15, 0).RotatedBy(MathHelper.TwoPi * (i / 5f)), ModContent.ProjectileType<ManisolBladeShadow>(), Projectile.damage, 0, Main.player[Projectile.owner].whoAmI);
        }

        /*public override void AI()
        {
            base.AI();
            if (AI_State == ActionState.Returning)
            {
                var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Mod.Find<ModProjectile>("ManisolBladeShadow").Type, Projectile.damage, 0, Projectile.owner, 0f, 0f);
                proj.rotation = Projectile.rotation;
            }
        }*/

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (npcStick == -1 && AI_State == ActionState.Thrown)
            {
                if (penerateCount < 5)
                {
                    penerateCount++;
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                    npcStick = target.whoAmI;
                    stickPosition = Projectile.position - target.position;
                    AI_State = ActionState.Stick;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
