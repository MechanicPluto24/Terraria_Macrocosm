using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class LHBMinigunProjectile : ChargedGunHeldProjectile
    {
        private const int windupFrames = 4; // number of windup animaton frames
        private const int shootFrames = 6;  // number of shooting animaton frames

        private const int startTicksPerFrame = 8; // tpf at the start of the animation
        private const int maxTicksPerFrame = 2;   // tpf cap after fullFireRateTime

        private const int windupTime = 45;        // ticks till start of shooting 
        private const int fullFireRateTime = 80;  // ticks to reach full fire rate 

        private const int fireRateStart = 6;      // fireRateFreqStart the ticks period between fires at the start of shooting
        private const int fireRateCap = 1;        // fireRateFreqMaxthe ticks period between fires after fullFireRateTime

        public ref float AI_Windup => ref Projectile.ai[0];

        public override float CircularHoldoutOffset => 32f;

        public override void SetProjectileStaticDefaults()
        {
            Main.projFrames[Type] = 10;
        }

        public override void SetProjectileDefaults()
        {

        }

        private bool CanShoot => AI_Windup >= windupTime;
        public override void ProjectileAI()
        {
            Animate();
            Shoot();
            Visuals();

            if (!Main.dedServ)
                PlaySounds();

            AI_Windup++;
        }

        private float WindupProgress => MathHelper.Clamp((AI_Windup / fullFireRateTime), 0, 1);
        private int WindupTicksPerFrame => (int)MathHelper.Clamp(MathHelper.Lerp(startTicksPerFrame, maxTicksPerFrame, WindupProgress), maxTicksPerFrame, startTicksPerFrame);
        private void Animate()
        {
            Projectile.frameCounter++;

            if (!CanShoot)
            {
                if (Projectile.frameCounter >= WindupTicksPerFrame)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Type] - shootFrames)
                        Projectile.frame = 0;
                }
            }
            else
            {
                if (Projectile.frameCounter >= WindupTicksPerFrame)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Type])
                        Projectile.frame = windupFrames;
                }
            }
        }

        private void Shoot()
        {
            if (CanShoot)
            {
                int damage = OwnerPlayer.GetWeaponDamage(OwnerPlayer.inventory[OwnerPlayer.selectedItem]); //makes the damage your weapon damage + the ammunition used.
                int projToShoot = ProjectileID.Bullet;
                float knockback = OwnerPlayer.inventory[OwnerPlayer.selectedItem].knockBack;

                if (StillInUse)
                {
                    if (!OwnerPlayer.PickAmmo(OwnerPlayer.inventory[OwnerPlayer.selectedItem], out projToShoot, out float speed, out damage, out knockback, out var usedAmmoItemId)) //uses ammunition from inventory
                        Projectile.Kill();
                }

                Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(40, 8 * Projectile.spriteDirection), Projectile.rotation);

                // gradually increase fire rate
                int fireFreq = (int)(MathHelper.Clamp(MathHelper.Lerp(fireRateStart, fireRateCap, ((AI_Windup - windupTime) / (fullFireRateTime - windupTime)) * OwnerPlayer.GetAttackSpeed(DamageClass.Ranged)), fireRateCap, fireRateStart));// Main.rand.NextBool()

                if (AI_Windup % fireFreq == 0 && Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), rotPoint, Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.ToRadians(14)) * 10f, projToShoot, damage, knockback, Projectile.owner, default, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));

                Projectile.position += (new Vector2(Main.rand.NextFloat(2.4f), Main.rand.NextFloat(0.4f))).RotatedBy(Projectile.rotation) * WindupProgress;

                #region Spawn bullet shells as gore
                if (!Main.dedServ && AI_Windup % (fireFreq * 1.5) == 0)
                {
                    Vector2 position = Projectile.Center - new Vector2(-20, 0) * Projectile.spriteDirection;
                    Vector2 velocity = new(1.2f * Projectile.spriteDirection, 4f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), position, velocity, ModContent.GoreType<MinigunShell>());
                }
                #endregion
            }
        }

        public void Visuals()
        {
            if (CanShoot)
                Lighting.AddLight(Projectile.position + Utility.PolarVector(80f, Projectile.rotation), TorchID.Torch);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawAnimated(lightColor, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 10));
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Ranged/LHBMinigunProjectile_Glow").Value;
            Projectile.DrawAnimatedExtra(glowmask, Color.White, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, new Vector2(5, 10));
        }

        private SlotId playingSoundId;
        private void PlaySounds()
        {
            if (AI_Windup == 0f)
            {
                playingSoundId = SoundEngine.PlaySound(SFX.MinigunWindup with
                {
                    Volume = 0.3f,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Projectile.position);
            }
            else if (AI_Windup == windupTime)
            {
                playingSoundId = SoundEngine.PlaySound(SFX.MinigunFire with
                {
                    Volume = 0.3f,
                    IsLooped = true,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                },
                Projectile.position);
            }

            if (SoundEngine.TryGetActiveSound(playingSoundId, out ActiveSound playingSound))
            {
                playingSound.Position = Projectile.position;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(playingSoundId, out ActiveSound playingSound))
                playingSound.Stop();
        }
    }
}
