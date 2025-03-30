using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class LuminitePebble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;
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
            Projectile.width = 16;
            Projectile.height = 16;
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
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Rectangle frame = texture.Frame(verticalFrames: 2, frameY: Projectile.frame);
                Vector2 drawPos = Projectile.oldPos[i] + frame.Size() / 2f - Main.screenPosition;
                float progress = (float)i / Projectile.oldPos.Length;
                Color trailColor = Projectile.GetAlpha(LuminiteSlime.EffectColor).WithAlpha(50) * (1f - progress) * 0.75f;
                float scale = Projectile.scale * Utility.QuadraticEaseIn(1f - progress) * 1.4f;
                if (i == 0) scale *= 1.1f;
                Main.spriteBatch.Draw(texture, drawPos, frame, trailColor * 0.6f, Projectile.oldRot[i], frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
            return true;
        }


        private bool spawned;
        private float baseShootSpeed = 12f;

        public override void AI()
        {
            if (!spawned)
            {
                baseShootSpeed = Projectile.velocity.Length();
                Projectile.frame = Main.rand.Next(0, 1);
                Projectile.rotation = Projectile.velocity.ToRotation();
                spawned = true;
            }

            bool hasTarget = TargetPlayer >= 0 && TargetPlayer < Main.maxPlayers;
            float shootDeviation = 0.5f;
            Projectile.alpha = 0;

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
            else if (!hasTarget && !Fall)
            {
                Fall = true;
                Projectile.netUpdate = true;
            }

            if (Fall)
            {
                Projectile.tileCollide = !WorldGen.SolidTile(Projectile.Center.ToTileCoordinates());
                Projectile.velocity.Y += hasTarget ? 0.1f : 0.3f;
                Projectile.rotation += 0.25f;
            }
            else
            {
                Projectile.rotation += 0.15f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.LunarOre, dustVelocity.X, dustVelocity.Y, newColor: Color.White);
            }
        }
    }
}
