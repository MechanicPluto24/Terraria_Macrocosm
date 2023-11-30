using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class SeleniteBeam : ModProjectile, IRangedProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        float trailMultiplier = 0f;

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (trailMultiplier < 1f)
                trailMultiplier += 0.01f;

            Lighting.AddLight(Projectile.Center, new Color(177, 230, 204).ToVector3() * 0.6f);
        }

        public override void OnKill(int timeLeft)
        {
            // TODO: sound

            for (int i = 0; i < 35; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<SeleniteBrightDust>(), Main.rand.NextVector2CircularEdge(10, 10) * Main.rand.NextFloat(1f), Scale: Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }
        }

        SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            float count = Projectile.velocity.LengthSquared() * trailMultiplier;

            for (int n = 1; n < count; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.4f;
                Color color = new Color(177, 230, 204) * (0.8f - (float)n / count);
                spriteBatch.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * 0.65f, Projectile.rotation, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }

    }
}
