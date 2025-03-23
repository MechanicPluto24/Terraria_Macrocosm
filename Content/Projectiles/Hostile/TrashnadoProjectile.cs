using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.NPCs.Enemies.Pollution;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class TrashnadoProjectile : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
        }

        private bool spawned;
        private TrashData trashData;
        public override void AI()
        {
            if (!spawned)
            {
                trashData = TrashData.RandomPool.Get();
                spawned = true;
            }

            Projectile.velocity.Y += MacrocosmSubworld.GetGravityMultiplier() / 4;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(10, 16); i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, trashData.DustType, newColor: trashData.Color);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(trashData.Texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Lerp(lightColor, Color.White, 1f - Projectile.alpha / 255f)), Projectile.rotation, trashData.Texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
