using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.NPCs.Enemies.Pollution;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

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
        bool init=false;
        string text;
        public override void AI()
        {
            if(!init)
            {
                text = Trashnado.GetTrash();
                init=true;
            }
            Projectile.velocity.Y += MacrocosmSubworld.GetGravityMultiplier() / 4;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(text).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Lerp(lightColor, Color.White, 1f - Projectile.alpha / 255f)), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
