using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns;

public class WaveRiflePurpleBolt : WaveGunBlueBolt
{
    public override int MaxPenetrate => 10;
    public override bool UseLocalImmunity => true;
    public override int LocalImmunityCooldown => -1;

    public override Color BeamColor => new(255, 150, 255, 0);
    public override Vector3 LightColor => new(1f, 0f, 1f);

    public override int DustCount => 80;
    public override int ParticleLightningCount => 32;
    public override float ParticleLightningSpread => 3f;
    public override float ParticleFlashScale => 0.1f;
    public override int TrailLength => 100;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        ParticleEffects(0.5f);
        //skipTrailUpdate = true;
        Projectile.damage = (int)(Projectile.damage * 0.9f);
    }
}
