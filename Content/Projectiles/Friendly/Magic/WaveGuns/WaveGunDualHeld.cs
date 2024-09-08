using Macrocosm.Common.Bases.Projectiles;
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

        public float MinCharge => MaxCharge * 0.2f;
        public ref float MaxCharge => ref Projectile.ai[0];
        public ref float AI_Timer => ref Projectile.ai[1];
        public ref float AI_Charge => ref Projectile.ai[2];

        public int fired;
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

                if (AI_Timer % currentItem.useTime == 0 && AI_Timer > 0)
                {
                    if (AI_Charge % 2 == 0)
                    {
                        redRotation += 0.3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 28f, ModContent.ProjectileType<WaveGunEnergyBolt>(), (int)(damage), knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Red);
                    }
                    else
                    {
                        blueRotation += 0.3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 28f, ModContent.ProjectileType<WaveGunEnergyBolt>(), (int)(damage), knockback, Projectile.owner, ai0: (float)WaveGunEnergyBolt.BeamVariant.Blue);
                    }

                    AI_Charge++;

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
