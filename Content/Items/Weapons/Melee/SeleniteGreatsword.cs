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
        public override bool RightClickUse => true;
        public override (float min, float max) ChargeBasedDamageRatio => (0.8f, 1.5f);
        public override int MaxCharge => 50;

        public override GreatswordSwingStyle SwingStyle => new SeleniteGreatswordSwingStyle();

        public override void SetStaticDefaults()
        {
            ItemID.Sets.UsesBetterMeleeItemLocation[Type] = true;
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.width = 78;
            Item.height = 80;
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
        }

        public override bool CanUseItemHeldProjectile(Player player)
        {
            if (player.AltFunction())
            {
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.useTime = 1;
                Item.useAnimation = 1;
                Item.UseSound = null;

            }
            else
            {
                Item.noUseGraphic = false;
                Item.noMelee = false;
                Item.useTime = 26;
                Item.useAnimation = 26;
                Item.UseSound = SoundID.Item1;
            }

            return base.CanUseItemHeldProjectile(player);
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

        public override void OnShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback, bool rightClick)
        {
            if (!rightClick)
            {
                Projectile.NewProjectileDirect(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwing>(), damage, knockback, player.whoAmI, ai0: player.direction * player.gravDir, ai1: player.itemAnimationMax, ai2: player.GetAdjustedItemScale(Item));
                //Projectile.NewProjectileDirect(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwing2>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, 12, -MathHelper.PiOver2 * 0.5f);
            }
        }

        public override Action<Player, int> OnRelease => (player, damage) =>
        {
            float charge = (damage - (Item.damage * ChargeBasedDamageRatio.min)) / ((Item.damage * ChargeBasedDamageRatio.max) - (Item.damage * ChargeBasedDamageRatio.min));
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
                Projectile swing = Projectile.NewProjectileDirect(null, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SeleniteGreatswordSwing>(), damage, 0, player.whoAmI, ai0: player.direction * player.gravDir, ai1: 24, ai2: player.GetAdjustedItemScale(Item));
                swing.scale = 1.15f - (0.2f * (1f - charge));
                swing.netUpdate = true;
            }
        };

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SeleniteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }

    public class SeleniteGreatswordSwingStyle : DefaultGreatswordSwingStyle
    {
        public override bool PreDrawSword(GreatswordHeldProjectile greatsword, Color lightColor, ref Color? drawColor)
        {
            drawColor = Color.Lerp(lightColor, new Color(130, 220, 199).WithOpacity(0.5f), Utility.QuadraticEaseIn(greatsword.Charge));
            return true;
        }

        public override void PostDrawSword(GreatswordHeldProjectile greatsword, Color lightColor)
        {
            if (greatsword.State == GreatswordHeldProjectile.GreatswordState.Charge)
            {
                Vector2 starPosition = greatsword.Projectile.Center + ((greatsword.Projectile.rotation - MathHelper.PiOver4) * greatsword.Player.direction + (greatsword.Player.direction == -1 ? MathHelper.Pi : 0f)).ToRotationVector2() * greatsword.SwordLength * 0.9f + new Vector2(greatsword.SwordWidth, 0) * greatsword.Player.direction;
                Utility.DrawStar(starPosition + Main.rand.NextVector2Circular(1, 1) - Main.screenPosition, 2, new Color(130, 220, 199).WithOpacity(1f - greatsword.Charge), new Vector2(1f, 3.2f) * Utility.QuadraticEaseIn(greatsword.Charge) * 0.4f, 0f, entity: true);

                // TODO: find a better way to do this lol
                if (greatsword.Charge == 1f && greatsword.Projectile.soundDelay != -1)
                {
                    SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.2f, Volume = 0.35f }, greatsword.Projectile.position);
                    greatsword.Projectile.soundDelay = -1;
                }
            }
        }
    }
}