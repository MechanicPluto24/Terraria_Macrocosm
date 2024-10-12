﻿using System;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class IlmeniteRegularProj : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        float trailMultiplier = 0f;
        int colourLerpProg = 0;
        public Color colour1 = new Color(188, 89, 134);
        public Color colour2 = new Color(33, 188, 190);

        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.extraUpdates += (int)Projectile.ai[1];
            Projectile.penetrate = (int)Projectile.ai[2];
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (trailMultiplier < 1f + (0.15f * Projectile.extraUpdates))
                trailMultiplier += 0.03f * (0.2f + Projectile.ai[0] * 0.9f);
            if (Projectile.ai[0] == 0) Lighting.AddLight(Projectile.Center, colour1.ToVector3());
            else if (Projectile.ai[0] == 1) Lighting.AddLight(Projectile.Center, colour2.ToVector3() * 1.25f);
            else
            {
                Lighting.AddLight(Projectile.Center, Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg/10f), 3)).ToVector3() * 1.5f);
                colourLerpProg++;
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
            Color color;
            if (Projectile.ai[0] == 0) color = colour1;
            else if (Projectile.ai[0] == 1) color = colour2;
            else
            {
                color = Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg / 10f), 3)) * 1.5f;
            }

            for (int n = 1; n < count; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.4f;
                color *= (1f - (float)n / count);
                Utility.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * (0.6f + Projectile.ai[0] * 0.15f), Projectile.rotation, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);

            for (int i = 0; i < 35; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<SeleniteBrightDust>(), Main.rand.NextVector2CircularEdge(10, 10) * Main.rand.NextFloat(1f), Scale: Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }

            float count = Projectile.oldVelocity.Length() * trailMultiplier;
            for (int i = 1; i < count; i++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * i * 0.4f;
                for (int j = 0; j < 2; j++)
                {
                    Dust dust = Dust.NewDustDirect(trailPosition, 1, 1, ModContent.DustType<SeleniteBrightDust>(), 0, 0, Scale: Main.rand.NextFloat(1f, 2f) * (1f - i / count));
                    dust.noGravity = true;
                }
            }
        }
    }
}
