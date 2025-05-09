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
    public class LuminiteShard : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public ref float AI_Timer => ref Projectile.ai[0];

        public int TargetPlayer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public ref float AI_FloatTime => ref Projectile.ai[2];
        public bool Fall { get; set; }

        private bool launched;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 5f, 1f, LuminiteSlime.EffectColor, LuminiteSlime.EffectColor * 0f);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Utility.DrawStar(Projectile.Center - Main.screenPosition, 1, LuminiteSlime.EffectColor, 0.6f, Projectile.rotation + MathHelper.PiOver2, entity: true);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }


        private bool spawned;
        private float baseShootSpeed = 12f;

        public override void AI()
        {
            if (!spawned)
            {
                baseShootSpeed = Projectile.velocity.Length();
                spawned = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            bool hasTarget = TargetPlayer >= 0 && TargetPlayer < Main.maxPlayers;
            float shootDeviation = 0.5f;

            if (hasTarget)
            {
                AI_Timer++;

                Player player = Main.player[TargetPlayer];
                if (!launched && AI_Timer >= AI_FloatTime && player.active && !player.dead && !player.longInvince)
                {
                    float aimAngle = (player.Center - Projectile.Center).ToRotation();
                    float shootSpeed = baseShootSpeed + Main.rand.NextFloat(-shootDeviation, shootDeviation);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Utility.PolarVector(shootSpeed, aimAngle), 0.08f);

                    if (AI_Timer >= 60)
                        launched = true;

                }
                else if (!Fall && AI_Timer > AI_FloatTime * Main.rand.NextFloat(1.5f, 3f))
                {
                    Fall = true;
                    Projectile.netUpdate = true;
                }
            }
            else if (!hasTarget)
            {
                Fall = true;
            }

            if (Fall)
            {
                Projectile.tileCollide = !WorldGen.SolidTile(Projectile.Center.ToTileCoordinates());
                Projectile.velocity.Y += hasTarget ? 0.1f : 0.3f;
            }
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }
        }
    }
}
