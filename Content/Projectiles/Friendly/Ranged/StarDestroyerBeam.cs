using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class StarDestroyerBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override string Texture => "Macrocosm/Assets/Textures/HighRes/Trace2";
        public Color colour = new Color(0, 0, 0, 0);
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.3f;
            Projectile.alpha = 255;

            ProjectileID.Sets.TrailCacheLength[Type] = 2;

            if (Main.rand.NextBool() == true)
                colour = new Color(100, 100, 255, 0);
            else
                colour = new Color(255, 180, 25, 0);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            if(Projectile.alpha > 0)
                Projectile.alpha -= 15;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, sourceRect, (colour) * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, new Vector2(Projectile.scale * 2f, Projectile.scale * 8f), SpriteEffects.None, 0);
            return false;
        }
    }
}

