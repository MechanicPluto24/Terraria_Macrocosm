using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Terraria.Audio;
using System.Collections;
using Macrocosm.Common.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Projectiles.Hostile;
public class ZombieBotanistVineBase : ModProjectile
{
    public override string Texture => "Macrocosm/Content/Projectiles/Hostile/ZombieBotanistVineBase";

    public override void SetDefaults()
    {
        Projectile.width = 15;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.tileCollide = false;
        Projectile.aiStyle = ProjAIStyleID.Arrow;
    }
}

public class ZombieBotanistVineTip : ModProjectile
{
    public override string Texture => "Macrocosm/Content/Projectiles/Hostile/ZombieBotanistVineTip";

    public override void SetDefaults()
    {
        Projectile.width = 15;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.tileCollide = true;
        Projectile.aiStyle = ProjAIStyleID.Vilethorn;
        AIType = ProjectileID.VilethornTip;
    }
}
