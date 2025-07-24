using Macrocosm.Common.Sets;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles;

public class BalanceGlobalProjectile : GlobalProjectile
{
    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (SubworldSystem.AnyActive<Macrocosm>())
            modifiers.SourceDamage *= ProjectileSets.DamageAdjustment[projectile.type];
    }
}
