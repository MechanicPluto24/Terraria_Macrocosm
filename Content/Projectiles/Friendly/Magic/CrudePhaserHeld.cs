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

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class CrudePhaserHeld : ChargedHeldProjectile
{
    public override string Texture => ModContent.GetInstance<CrudePhaser>().Texture;

    private int timer;
    private float gunRotation;

    protected override bool StillInUse => timer <= 24 && !Player.noItems && !Player.CCed && !Player.dead;
    public override float CircularHoldoutOffset => 4f;
    public override bool ShouldUpdateAimRotation => true;

    public override void SetProjectileDefaults()
    {
        Projectile.scale = 1f;
    }

    public override void ProjectileAI()
    {
        gunRotation *= 0.86f;

        if (Player.whoAmI == Main.myPlayer && (timer == 0 || timer == 7 || timer == 14))
        {
            Item currentItem = Player.CurrentItem();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX * Player.direction);
            Vector2 velocity = direction.RotatedByRandom(timer == 0 ? 0.02f : 0.05f) * (currentItem.shootSpeed / 5f) * Main.rand.NextFloat(0.92f, 1.08f);
            Vector2 position = Projectile.Center + direction * 20f + direction.RotatedBy(MathHelper.PiOver2) * Projectile.spriteDirection * -2f;
            bool phasingShot = timer == 0;

            Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                position,
                velocity,
                ModContent.ProjectileType<CrudePhaserBolt>(),
                Player.GetWeaponDamage(currentItem),
                currentItem.knockBack,
                Projectile.owner,
                phasingShot ? 1f : 0f
            );

            SoundEngine.PlaySound(SFX.WaveGunShot with { Volume = 0.45f, Pitch = phasingShot ? -0.2f : 0.1f, PitchVariance = 0.15f }, position);

            if (ClientConfig.Instance.GunRecoilEffects)
                gunRotation += phasingShot ? 0.26f : 0.18f;
        }

        timer++;
    }

    public override bool PreDraw(Player player, ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX * Projectile.spriteDirection);
        Vector2 drawPosition = Projectile.Center + direction * 4f - Main.screenPosition;
        SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Main.EntitySpriteDraw(
            texture,
            drawPosition,
            null,
            lightColor,
            Projectile.rotation - gunRotation * Projectile.spriteDirection,
            texture.Size() / 2f,
            Projectile.scale,
            effects,
            0f
        );

        return false;
    }
}
