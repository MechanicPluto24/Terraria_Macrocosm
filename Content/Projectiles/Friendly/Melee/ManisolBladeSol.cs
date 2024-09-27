using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ManisolBladeSol : ManisolBladeBase
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            dustType = DustID.OrangeTorch;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (npcStick == -1 && AI_State == ActionState.Thrown)
            {
                Projectile.velocity = Vector2.Zero;
                npcStick = target.whoAmI;
                stickPosition = Projectile.position - target.position;
                AI_State = ActionState.Stick;
            }
        }

        public override void OnReturn()
        {
            Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ManisolBladeSolExplosion>(), Projectile.damage, 12f, Main.myPlayer, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300, false);
        }
    }
}
