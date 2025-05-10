using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class LuminiteBolt : ModProjectile
    {
        // The targeted NPC whoAmI
        private int targetNPC;

        // The targeted NPC 
        private NPC TargetNPC => Main.npc[targetNPC];
        private float originalSpeed;

        public override string Texture => Macrocosm.EmptyTexPath;

        public float Strenght
        {
            get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
        }

        float trailMultiplier = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
            ProjectileSets.HitsTiles[Type] = true;
        }

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

            Projectile.CritChance = 16;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        bool spawned = false;
        public override void AI()
        {
            if (!spawned)
            {
                originalSpeed = Projectile.velocity.Length();
                spawned = true;
            }
            float homingDistance = 200f;
            float closestDistance = homingDistance;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile))
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetNPC = npc.whoAmI;
                    }
                }
            }
            if (TargetNPC is not null && Vector2.Distance(Projectile.Center, TargetNPC.Center) < homingDistance && TargetNPC.CanBeChasedBy(Projectile))
            {
                Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, 0.015f);
            }






            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (trailMultiplier < 1f)
                trailMultiplier += 0.006f;

            Lighting.AddLight(Projectile.Center, new Color(177, 230, 204).ToVector3() * 0.6f);
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(64, 64);
            Projectile.knockBack = 6f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<LuminiteBrightDust>(), Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(0.6f, 1.1f), Alpha: 127);
                dust.noGravity = true;
            }

            for (int i = 0; i < 60; i++)
            {
                Vector2 velocity = i % 2 == 0 ? Main.rand.NextVector2Circular(2, 9) : Main.rand.NextVector2Circular(9, 2);
                Particle.Create<TintableSpark>((p) =>
                {
                    p.Position = Projectile.Center + Projectile.oldVelocity;
                    p.Velocity = velocity;
                    p.Scale = new(5f);
                    p.Rotation = 0f;
                    p.Color = new List<Color>() {
                        new(44, 209, 147),
                        new(0, 141, 92)
                    }.GetRandom();
                });
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
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.8f;
                Color color = new Color(44, 209, 147) * (0.8f - n / count);
                Utility.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * 0.65f, Projectile.rotation, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }
    }
}
