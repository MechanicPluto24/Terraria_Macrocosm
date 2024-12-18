using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ProcellarumExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public ref float Strength => ref Projectile.ai[0];
        public bool ApplyDebuff
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.knockBack = 8f;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.hide = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(ApplyDebuff)
                target.AddBuff<ProcellarumLightningMark>(3 * 60);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if(ApplyDebuff)
                target.AddBuff<ProcellarumLightningMark>(3 * 60);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            SoundEngine.PlaySound(SoundID.Thunder, Projectile.position);

            for (int i = 0; i < (int)(140 * Strength); i++)
            {
                Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = new Vector2(12).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.8f);
                    p.Scale = new(Main.rand.NextFloat(1f));
                    p.Rotation = p.Velocity.ToRotation();
                    p.Color = new List<Color>() {
                        new(77, 99, 124, 0),
                        new(90, 83, 92, 0),
                        new(232, 239, 255, 0)
                    }.GetRandom();
                    p.OutlineColor = p.Color * 0.2f;
                });
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center;
                p.Scale = new(0.2f);
                p.ScaleVelocity = new(0.25f * Strength);
                p.Color = new Color(232, 239, 255);
            });
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithAlpha(127);
    }
}
