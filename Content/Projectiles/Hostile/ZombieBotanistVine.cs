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

    public ref float vineCount => ref Projectile.ai[0];
    public ref float vineLimit => ref Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.width = 15;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        if (!(vineCount >= vineLimit))
        {
            int branchChance = Main.rand.Next(1, 10);
            if (branchChance == 1)
            { 
                // spawn two branches make one shorter or longer
            }
            else
            {
                // normal vine extension
            }
        }
        else
        {
            //spawn vine tip
        }
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
        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        // stand still (maybe delete this and use some regular vanilla AI pretty sure they got some AI template for things that stand still
    }
}
