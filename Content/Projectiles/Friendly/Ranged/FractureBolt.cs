using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Bases.Projectiles;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class FractureBolt : RailgunProjectile 
{
    public override Color beamColor => new Color (255, 240, 255);
    public override int dustType => ModContent.DustType<QuartzDust>();
    public override void HitTileBehaviour()
    {
        for(int i =0; i<3; i++)
        {
                Projectile.NewProjectileDirect(
                Projectile.GetSource_FromAI(),
                beamEnd- Projectile.velocity.SafeNormalize(Vector2.UnitX),
                -Projectile.velocity.RotatedByRandom(0.4f),
                ModContent.ProjectileType<FractureBoltFracture>(),
                Projectile.damage / 3,
                knockback: 1f,
                owner: Main.myPlayer
            );
        }
    }

}
public class FractureBoltFracture : RailgunProjectile 
{
    public override Color beamColor => new Color (255, 240, 255);
    public override int dustType => -1;
    public override float visualScale => 0.5f;
    

}
