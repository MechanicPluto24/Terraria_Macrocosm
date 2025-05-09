using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    [LegacyName("ArtemiteGreatsword")]
    public class SeleniteGreatsword : GreatswordHeldProjectileItem
    {
        public override Vector2 SpriteHandlePosition => new(23, 68);
        public override UseBehavior LeftClickBehavior => UseBehavior.Swing;
        public override UseBehavior RightClickBehavior => UseBehavior.Charge;
        public override (float min, float max) ChargeBasedDamageRatio => base.ChargeBasedDamageRatio;
        public override int MaxCharge => 50;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.width = 78;
            Item.height = 80;
            Item.damage = 300;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.UseSound = SoundID.Item1;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.Create<SeleniteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = new(1f);
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        public override void OnMaxCharge(GreatswordHeldProjectile proj, Player player)
        {
            if(proj.Projectile.soundDelay != -1)
            {
                SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.2f, Volume = 0.35f }, proj.Projectile.position);
                proj.Projectile.soundDelay = -1;
            }
        }

        public override void OnRelease(GreatswordHeldProjectile proj, Player player, int damage, float charge)
        {
            if (charge > 0.65f)
            {
                Projectile swing = Projectile.NewProjectileDirect(null, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwing>(), 0, 0, player.whoAmI, ai0: player.direction * player.gravDir, ai1: 24, ai2: player.GetAdjustedItemScale(Item));
                Projectile swing2 = Projectile.NewProjectileDirect(null, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwingEmpowered>(), damage, 5, player.whoAmI, ai0: player.direction * player.gravDir, ai1: 24, ai2: player.GetAdjustedItemScale(Item));
                swing2.scale = charge;
                swing.alpha = (byte)(255f * (1f - charge));
                swing2.netUpdate = true;
            }
            else
            {
                Projectile swing = Projectile.NewProjectileDirect(null, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwing>(), damage, 0, player.whoAmI, ai0: player.direction * player.gravDir, ai1: 26, ai2: player.GetAdjustedItemScale(Item));
                swing.scale = 1.15f - (0.2f * (1f - charge));
                swing.netUpdate = true;
            }
        }

        public override bool PreDrawSword(GreatswordHeldProjectile greatsword, Color lightColor, ref Color? drawColor)
        {
            drawColor = Color.Lerp(lightColor, new Color(130, 220, 199).WithOpacity(0.5f), Utility.QuadraticEaseIn(greatsword.Charge));
            return true;
        }

        public override void PostDrawSword(GreatswordHeldProjectile proj, Color lightColor)
        {
            if (proj.State == GreatswordHeldProjectile.GreatswordState.Charge)
            {
                Vector2 starPosition = proj.Projectile.Center + ((proj.Projectile.rotation - MathHelper.PiOver4) * proj.Player.direction + (proj.Player.direction == -1 ? MathHelper.Pi : 0f)).ToRotationVector2() * proj.Sword.SwordLength * 0.9f + new Vector2(proj.Sword.SwordWidth, 0) * proj.Player.direction;
                Utility.DrawStar(starPosition + Main.rand.NextVector2Circular(1, 1) - Main.screenPosition, 2, new Color(130, 220, 199).WithOpacity(1f - proj.Charge), new Vector2(1f, 3.2f) * Utility.QuadraticEaseIn(proj.Charge) * 0.4f, 0f, entity: true);

            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SeleniteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}