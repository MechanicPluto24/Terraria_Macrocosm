using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class MoonSword : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.crit = 4;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 1;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.shoot = ModContent.ProjectileType<MoonSwordProjectile>();
            Item.shootSpeed = 20;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<MoonSwordSwing>(), damage, knockback, player.whoAmI, (float)player.direction * player.gravDir, 15); //, player.GetAdjustedItemScale(Item));
            return true;
        }

    }


    /*
    public class ChampionsBladeProjectile : ModProjectile
    {
        protected virtual float HoldoutRangeMin => 25;
        protected virtual float HoldoutRangeMax => 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion's Blade Projectile");
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 999;
            Projectile.timeLeft = 99999;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            #region Variables
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            float swingSpeed = 11.5f;
            #endregion

            #region Duration
            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }
            #endregion

            #region Movement
            Projectile.Center = player.MountedCenter;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            #endregion

            #region Sprite Rotation
            if (Main.MouseWorld.X > player.position.X)
            {
                Projectile.rotation += MathHelper.ToRadians(swingSpeed);

                Projectile.spriteDirection = 1;
                player.direction = 1;

                #region Offset
                DrawOffsetX = -45;

                DrawOriginOffsetX = 45;
                DrawOriginOffsetY = -45;

                Projectile.position += new Vector2(-1, 0);
                #endregion
            }
            else if (Main.MouseWorld.X < player.position.X)
            {
                Projectile.rotation -= MathHelper.ToRadians(swingSpeed);

                Projectile.spriteDirection = -1;
                player.direction = -1;

                #region Offset
                DrawOffsetX = 45;

                DrawOriginOffsetX = -45;
                DrawOriginOffsetY = -45;

                Projectile.position += new Vector2(1, 0);
                #endregion
            }
            #endregion
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 150;
            hitbox.Height = 200;

            if (Main.player[Projectile.owner].direction == 1)
            {
                hitbox.Offset(15, -75);
            }
            else if (Main.player[Projectile.owner].direction == -1)
            {
                hitbox.Offset(-15, -75);
            }
        }
    }
    */
}