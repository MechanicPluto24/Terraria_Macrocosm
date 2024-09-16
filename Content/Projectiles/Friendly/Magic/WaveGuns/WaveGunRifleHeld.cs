using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
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
    public class WaveGunRifleHeld : ChargedHeldProjectile
    {
        public override string Texture => base.Texture;

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

        public override float CircularHoldoutOffset => 8f;
        protected override bool StillInUse => base.StillInUse || itemUseTime > 0;
        public override bool ShouldUpdateAimRotation => true;

        public override void SetProjectileStaticDefaults()
        {
        }

        public override void SetProjectileDefaults()
        {
            Projectile.scale = 0.8f;
        }

        public override void ProjectileAI()
        {
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
            Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (Player.whoAmI == Main.myPlayer)
            {
                Item currentItem = Player.CurrentItem();
                int damage = Player.GetWeaponDamage(currentItem);
                float knockback = currentItem.knockBack;

                if (AI_Timer % AI_FireRate == 0)
                {
                    Vector2 position = Projectile.Center + Projectile.velocity * 5;
                    Vector2 velocity = Vector2.Normalize(Projectile.velocity) * (currentItem.shootSpeed / ContentSamples.ProjectilesByType[ModContent.ProjectileType<WaveGunEnergyBolt>()].extraUpdates);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<WaveGunEnergyBolt>(), (int)(damage * 1.4), knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Purple);

                    if (!Player.CheckMana(currentItem.mana, pay: true))
                        Projectile.Kill();

                    SoundEngine.PlaySound(SFX.WaveGunShotRifle, Projectile.position);
                }

                AI_Timer++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(10, 0), Projectile.rotation);
            spriteBatch.Draw(texture, rotPoint - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.Pi / 32, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }
}
