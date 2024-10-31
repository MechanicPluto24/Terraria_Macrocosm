using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class IlmeniteExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public float Strength
        {
            get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
        }

        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(50, 50);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.CritChance = 16;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Size *= 1f + (0.5f * Projectile.ai[0]);
        }

        SpriteBatchState state;
        public override void AI(){
            for (int i = 0; i < 10 * Strength; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Vector2 velocty = new Vector2(5 * Main.rand.NextFloat() * Strength).RotatedBy((position - Projectile.Center).ToRotation() - MathHelper.PiOver2);
                Particle.Create<LunarRustStar>((p) =>
                {
                    p.Position = position;
                    p.Velocity = Vector2.Zero;
                    p.Rotation = Utility.RandomRotation();
                    p.Scale = new Vector2(1f * (0.6f + Strength * 0.15f)) * Main.rand.NextFloat(0.5f, 1.2f);
                });
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            

            return false;
        }
    }
}
