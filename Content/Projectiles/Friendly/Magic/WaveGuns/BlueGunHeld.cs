using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class BlueGunHeld : ChargedHeldProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public ref float AI_Timer => ref Projectile.ai[1];
        public ref float AI_Charge => ref Projectile.ai[2];

        public override float CircularHoldoutOffset => 4f;

        protected override bool StillInUse => base.StillInUse || itemUseTime > 0;

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
                if (AI_Timer % currentItem.useTime == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 / 9)) * 25f, ModContent.ProjectileType<BlueEnergyBolt>(), damage, knockback, Projectile.owner);
                    GunRotation += 0.3f;
                }

                AI_Timer++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            Texture2D texture2 = ModContent.Request<Texture2D>("Macrocosm/Content/Items/Weapons/Magic/WaveGunBlue").Value;
            Vector2 rotPoint2 = Utility.RotatingPoint(Projectile.Center, new Vector2(15, 0), Projectile.rotation);
            spriteBatch.Draw(texture2, rotPoint2 - Main.screenPosition, null, lightColor, Projectile.rotation + GunRotation, texture2.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            return false;
        }
    }
}
