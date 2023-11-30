using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LuminiteStar : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public ref float AI_Timer => ref Projectile.ai[0];

        public int TargetPlayer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public bool Fall
        {
            get => Projectile.ai[2] > 0f;
            set => Projectile.ai[2] = value ? 1f : 0f;
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

            Projectile.tileCollide = true;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 5f, 1f, new Color(98, 211, 168, 255), new Color(98, 211, 168, 0));

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.spriteBatch.DrawStar(Projectile.Center - Main.screenPosition, 2, new Color(89, 151, 193), 0.4f, Projectile.rotation, entity: true);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);


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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float aimAngle = (Main.player[TargetPlayer].Center - Projectile.Center).ToRotation();
                    float shootSpeed = baseShootSpeed + Main.rand.NextFloat(-shootDeviation, shootDeviation);
                    Projectile.velocity = Utility.PolarVector(shootSpeed, aimAngle);

                    Projectile.netUpdate = true;
                }          
            }
            else if (AI_Timer > timeToShoot)
            {
                Vector2 direction = (Main.player[TargetPlayer].Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * baseShootSpeed, 0.005f);

                if (!Fall && AI_Timer > timeToShoot * Main.rand.NextFloat(2.5f, 4f) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Fall = true;
                    Projectile.netUpdate = true;
                }
            }

            if (Fall)
            {
                Projectile.tileCollide = !WorldGen.SolidTile(Projectile.Center.ToTileCoordinates());
                Projectile.velocity.Y += 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
