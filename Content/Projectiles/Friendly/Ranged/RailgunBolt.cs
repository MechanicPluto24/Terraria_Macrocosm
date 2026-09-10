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

public class RailgunBolt : RailgunProjectile
{
    public override Color beamColor => new Color (240, 240, 240);
    public override int dustType => DustID.WhiteTorch;

}
