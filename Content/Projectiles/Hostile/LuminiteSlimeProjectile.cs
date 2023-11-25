using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LuminiteSlimeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public ref float AI_Timer => ref Projectile.ai[0];

        public int TargetPlayer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 4f, 1f, new Color(98, 211, 168, 255) * lightColor.GetLuminanceNTSC(), new Color(98, 211, 168, 1));

            // draw circular glow
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle6").Value;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, new Color(89, 151, 193) * lightColor.GetLuminanceNTSC(), 0f, glow.Size() / 2, 0.0375f, SpriteEffects.None);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * lightColor.GetLuminanceNTSC(), 0f, TextureAssets.Projectile[Type].Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }

        public override void AI()
        {
            float timeToShoot = 40;
            float baseShootSpeed = 12f;
            float shootDeviation = 0.5f;

            AI_Timer++;
            if (AI_Timer == timeToShoot)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;

                float aimAngle = (Main.player[TargetPlayer].Center - Projectile.Center).ToRotation();
                float shootSpeed = baseShootSpeed + Main.rand.NextFloat(-shootDeviation, shootDeviation);
                Projectile.velocity = Utility.PolarVector(shootSpeed, aimAngle);

                Projectile.netUpdate = true;
            }
        }
    }
}
