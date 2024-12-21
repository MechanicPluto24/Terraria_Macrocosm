using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public abstract class BaseMeteor : ModProjectile
    {
        public static List<LocalizedText> DeathMessages { get; } = [];

        protected float screenshakeMaxDist;
        protected float screenshakeIntensity;

        protected float rotationMultiplier;
        protected int blastRadius = 128;

        private int collisionTileType = -1;

        public override void Load()
        {
            for (int i = 0; i < Utility.FindAllLocalizationThatStartsWith("Mods.Macrocosm.DeathMessages.Meteor").Length; i++)
                DeathMessages.Add(Language.GetOrRegister($"Mods.Macrocosm.DeathMessages.Meteor.Message{i}"));
        }

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
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Type == info.DamageSource.SourceProjectileType)
            {
                // Can't use info.DamageSource = PlayerDeathReason.ByCustomReason(...) here:
                // HurtInfo is a value type and a DamageSource reassignment won't be reflected outside this method
                // Called on the hit player client, will be synced and message will be broadcasted by the server 
                info.DamageSource.SourceCustomReason = DeathMessages.GetRandom().Format(target.name);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // handled by clients 
            if (Main.netMode != NetmodeID.Server)
            {
                ImpactEffects(collisionTileType);
                ImpactScreenshake();
                ImpactSound();
            }

            // handled by server 
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                SpawnItems();
            }
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(blastRadius, blastRadius);
            Projectile.knockBack = 12f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;

            if (Main.tile[Projectile.Center.ToTileCoordinates()].HasTile)
                collisionTileType = Main.tile[Projectile.Center.ToTileCoordinates()].TileType;

            Projectile.velocity *= 0f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public sealed override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            AI_Rotation();
            MeteorAI();
        }

        public virtual void AI_Rotation()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * rotationMultiplier * Projectile.direction;
        }

        public virtual void MeteorAI() { }

        public virtual void ImpactEffects(int collisionTileType) { }
        public virtual void SpawnItems() { }

        public virtual void ImpactScreenshake()
        {
            for (int i = 0; i < 255; i++)
            {
                Player player = Main.player[i];
                if (player.active)
                {
                    float distance = Vector2.Distance(player.Center, Projectile.Center);
                    if (distance < screenshakeMaxDist)
                    {
                        player.AddScreenshake(screenshakeIntensity - distance / screenshakeMaxDist * screenshakeIntensity, context: FullName + Projectile.whoAmI.ToString());
                    }
                }
            }
        }

        public virtual void ImpactSound()
        {
            // broken, for some reason
            SoundEngine.PlaySound(SFX.BigExplosion with { Volume = 0.4f, PitchRange = (-0.5f, 0.5f) }, Projectile.Center);
        }
    }
}
