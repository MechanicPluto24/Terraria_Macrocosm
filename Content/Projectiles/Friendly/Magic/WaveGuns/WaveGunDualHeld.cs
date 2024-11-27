using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class WaveGunDualHeld : ChargedHeldProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public int AI_FireRate
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int AI_Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int AI_ShotCount
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }


        public int fired;
        public override float CircularHoldoutOffset => 8f;

        protected override bool StillInUse => base.StillInUse || itemUseTimer > 0;
        public override bool ShouldUpdateAimRotation => true;

        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {
            Projectile.scale = 0.8f;
        }

        private float blueRotation = 0f;
        private float redRotation = 0f;

        public override void ProjectileAI()
        {
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            blueRotation -= 0.07f;
            redRotation -= 0.07f;

            if (blueRotation < 0f)
                blueRotation = 0f;

            if (redRotation < 0f)
                redRotation = 0f;

            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();

                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;

                if (AI_Timer % AI_FireRate == 0)
                {
                    Vector2 position = Projectile.Center - Projectile.velocity * 5;
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity) * (currentItem.shootSpeed / ContentSamples.ProjectilesByType[ModContent.ProjectileType<WaveGunEnergyBolt>()].extraUpdates);

                    if (AI_ShotCount % 2 == 0)
                    {
                        if (ClientConfig.Instance.GunRecoilEffects)
                            redRotation += 0.3f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<WaveGunEnergyBolt>(), (int)(damage), knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Red);
                    }
                    else
                    {
                        if (ClientConfig.Instance.GunRecoilEffects)
                            blueRotation += 0.3f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<WaveGunEnergyBolt>(), (int)(damage), knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Blue);
                    }

                    if (!Player.CheckMana(currentItem.mana, pay: true))
                        Projectile.Kill();

                    AI_ShotCount++;

                    SoundEngine.PlaySound(SFX.WaveGunShot, Projectile.position);
                }

                AI_Timer++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Texture2D redTexture = TextureAssets.Item[ModContent.GetInstance<WaveGunBlue>().Type].Value;
            Vector2 rotPoint2 = Utility.RotatingPoint(Projectile.Center, new Vector2(15, 0), Projectile.rotation);
            spriteBatch.Draw(redTexture, rotPoint2 - Main.screenPosition, null, lightColor, Projectile.rotation + blueRotation, redTexture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            Texture2D blueTexture = TextureAssets.Item[ModContent.GetInstance<WaveGunRed>().Type].Value;
            Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(10, 0), Projectile.rotation);
            spriteBatch.Draw(blueTexture, rotPoint - Main.screenPosition, null, lightColor, Projectile.rotation + redRotation, blueTexture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }
}
