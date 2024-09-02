using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Bases.Projectiles
{
    public abstract class BaseMeteor : ModProjectile
    {
        public static List<LocalizedText> DeathMessages { get; } = [];

        public float ScreenshakeMaxDist;
        public float ScreenshakeIntensity;

        public float RotationMultiplier;
        public float BlastRadiusMultiplier = 1f;

        public override void Load()
        {
            for (int i = 0; i < Utility.FindAllThatStartWith("Mods.Macrocosm.DeathMessages.Meteor").Length; i++)
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
            if(Type == info.DamageSource.SourceProjectileType)
            {
                // Can't use info.DamageSource = PlayerDeathReason.ByCustomReason(...) here:
                // HurtInfo is a value type and a DamageSource reassignment won't be reflected outside this method
                info.DamageSource.SourceCustomReason = DeathMessages.GetRandom().Format(target.name);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // handled by clients 
            if (Main.netMode != NetmodeID.Server)
            {
                ImpactEffects();
                ImpactScreenshake();
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

        public sealed override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            AI_Rotation();
            MeteorAI();
        }

        public virtual void AI_Rotation()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * RotationMultiplier * Projectile.direction;
        }

        public virtual void MeteorAI() { }

        public virtual void ImpactEffects() { }
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
    }
}
