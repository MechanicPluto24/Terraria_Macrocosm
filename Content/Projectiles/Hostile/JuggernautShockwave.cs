using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class JuggernautShockwave : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
     
        }

        public ref float AI_Timer => ref Projectile.ai[0];

     

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 128;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;

        int Timer=0;
        public override void AI()
        {
            if(Timer==0){
                Vector2 start = new Vector2(Projectile.position.X,Projectile.position.Y+112);
                start.X += 16 * Projectile.velocity.X;
                int tries = 0;
                int x = (int)(start.X / 16);
                int y = (int)(start.Y / 16);
                while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                {
                    y++;
                    start.Y = y * 16;
                }
                while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && tries < 20)
                {
                    tries++;
                    y--;
                    start.Y = y * 16;
                }
                if (tries >= 20)
                    Projectile.Kill();
                Projectile.position=new Vector2(start.X,start.Y-112);

                for (int i = 0; i < 4; i++)
                {
                    int dust = Dust.NewDust(Projectile.Bottom, Projectile.width, 2, DustID.Blood, Projectile.velocity.X * 0.5f,Projectile.velocity.Y * 0.5f, Scale: 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y = -Main.rand.Next(6, 12);
                }
                for (int i = 0; i < 8; i++)
                {
                    int dust = Dust.NewDust(Projectile.Bottom, Projectile.width, 2, ModContent.DustType<RegolithDust>(), Projectile.velocity.X * 0.5f,Projectile.velocity.Y * 0.5f, Scale: 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y = -Main.rand.Next(6, 12);
                }
            }
        }

    }
}
