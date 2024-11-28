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
    public class WaveGunRedHeld : ChargedHeldProjectile
    {
        public override string Texture => ModContent.GetInstance<WaveGunRed>().Texture;

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

        public override float CircularHoldoutOffset => 4f;
        protected override bool StillInUse => base.StillInUse || itemUseTimer > 0;
        public override bool ShouldUpdateAimRotation => true;

        private float GunRotation = 0f;

        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {
            Projectile.scale = 0.8f;
        }

        public override void ProjectileAI()
        {
            GunRotation -= 0.07f;

            if (GunRotation < 0f)
                GunRotation = 0f;

            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();

                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;
                if (AI_Timer % AI_FireRate == 0)
                {
                    Vector2 position = Projectile.Center - Projectile.velocity * 5 + new Vector2(0, 2).RotatedBy(Projectile.velocity.ToRotation());
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 / 9)) * (currentItem.shootSpeed / ContentSamples.ProjectilesByType[ModContent.ProjectileType<WaveGunEnergyBolt>()].extraUpdates);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<WaveGunEnergyBolt>(), damage, knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Red);

                    if (!Player.CheckMana(currentItem.mana, pay: true))
                        Projectile.Kill();

                    SoundEngine.PlaySound(SFX.WaveGunShot, Projectile.position);

                    if (ClientConfig.Instance.GunRecoilEffects)
                        GunRotation += 0.3f;
                }

                AI_Timer++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(15, 0), Projectile.rotation);
            spriteBatch.Draw(texture, rotPoint - Main.screenPosition, null, lightColor, Projectile.rotation + GunRotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }
}
