using Macrocosm.Common.Global.Projectiles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public abstract class BaseMeteor : ModProjectile
    {
        public int Width;
        public int Height;

        public int Damage;

        public float ScreenshakeMaxDist;
        public float ScreenshakeIntensity;

        public float RotationMultiplier;
        public float BlastRadiusMultiplier = 1f;

        public int DustType = -1;
        public int ImpactDustCount;
        public Vector2 ImpactDustSpeed;

        public float DustScaleMin;
        public float DustScaleMax;

        public int AI_DustChanceDenominator;

        public int DebrisType = -1;
        public int DebrisCount;
        public Vector2 DebrisVelocity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.width = Width;
            Projectile.height = Height;
            Projectile.damage = Damage;
        }

        public override void OnKill(int timeLeft)
        {
            // handled by clients 
            if (Main.netMode != NetmodeID.Server)
            {
                SpawnImpactDusts();
                SpawnDebris();
                ImpactSounds();
            }

            // handled by server 
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                SpawnItems();
                ImpactScreenshake();
            }
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize((int)(Projectile.width * BlastRadiusMultiplier), (int)(Projectile.height * BlastRadiusMultiplier));
            Projectile.knockBack = 12f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 3;
            Projectile.velocity *= 0f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        override public void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            AI_Rotation();
            AI_SpawnDusts();
            ExtraAI();
        }

        public virtual void SpawnImpactDusts()
        {
            if (DustType < 0)
                return;

            SpawnImpactDusts(DustType);
        }

        public void SpawnImpactDusts(int dustType, bool noGravity = false)
        {
            for (int i = 0; i < ImpactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    new Vector2(Projectile.Center.X, Projectile.Center.Y + 0.25f * Projectile.height),
                    Width,
                    Height,
                    dustType,
                    Main.rand.NextFloat(-ImpactDustSpeed.X, ImpactDustSpeed.X),
                    Main.rand.NextFloat(0f, -ImpactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = noGravity;
            }
        }

        public virtual void SpawnDebris()
        {
            if (DebrisType < 0)
                return;

            for (int i = 0; i < DebrisCount; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                new Vector2(DebrisVelocity.X * Main.rand.NextFloat(-6f, 6f), DebrisVelocity.Y * Main.rand.NextFloat(-4f, -1f)),
                DebrisType, 0, 0f, 255);
            }
        }

        public virtual void ImpactSounds() { }
        public virtual void SpawnItems() { }

        public virtual void ImpactScreenshake()
        {
            for (int i = 0; i < 255; i++)
            {
                Player player = Main.player[i];
                if (player.active)
                {
                    float distance = Vector2.Distance(player.Center, Projectile.Center);
                    if (distance < ScreenshakeMaxDist)
                    {
                        player.AddScreenshake(ScreenshakeIntensity - distance / ScreenshakeMaxDist * ScreenshakeIntensity, context: FullName + Projectile.whoAmI.ToString());
                    }
                }
            }
        }

        public virtual void AI_Rotation()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * RotationMultiplier * Projectile.direction;

        }

        /// <summary> Override for custom dusts spawning </summary>
        public virtual void AI_SpawnDusts()
        {
            if (DustType == -1)
                return;

            AI_SpawnDusts(DustType);
        }

        /// <summary> Call for custom dust types, different from the DustType property </summary>
        public void AI_SpawnDusts(int dustType)
        {
            if (Main.rand.NextBool(AI_DustChanceDenominator))
            {
                Dust dust = Dust.NewDustDirect(
                        new Vector2(Projectile.position.X, Projectile.position.Y),
                        Projectile.width,
                        Projectile.height,
                        dustType,
                        0f,
                        0f,
                        Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                    );

                dust.noGravity = true;
            }
        }

        /// <summary> Use for special AI </summary>
        public virtual void ExtraAI() { }

    }
}
