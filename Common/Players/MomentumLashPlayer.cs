using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

public class MomentumLashPlayer : ModPlayer
{
    public bool MomentumLash { get; set; }

    private float momentumBonus;
    private int decreaseTick;

    public override void ResetEffects()
    {
        MomentumLash = false;
    }

    public override void PostUpdateEquips()
    {
        if (MomentumLash)
        {
            if (decreaseTick < 60 && momentumBonus > 0)
            {
                decreaseTick++;
            }

            if (decreaseTick >= 60)
            {
                momentumBonus -= 0.01f;
                decreaseTick = 45;
            }

            Player.GetDamage<SummonDamageClass>() += momentumBonus;
            Player.GetAttackSpeed<SummonDamageClass>() += momentumBonus;
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (MomentumLash && ProjectileID.Sets.IsAWhip[proj.type] == true && !target.friendly && target.type != NPCID.TargetDummy)
        {
            if (momentumBonus < 0.18f)
                momentumBonus += 0.01f;

            decreaseTick = 0;
        }
    }
}
