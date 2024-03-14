using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
	public class ArtemiteGreatsword : GreatswordHeldProjectileItem
	{
		public override Vector2 SpriteHandlePosition => new(23, 68);

        public override int SwingEffectType => ModContent.ProjectileType<ArtemiteGreatswordSwing>();
		public override bool RightClickUse => true;

        public override void SetStaticDefaults()
		{

		}

		public override void SetDefaultsHeldProjectile()
		{
			Item.damage = 225;
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

        public override void OnShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback, bool rightClick)
        {
			if (!rightClick)
			{
                Projectile.NewProjectileDirect(source, player.MountedCenter, new Vector2(player.direction, 0f), SwingEffectType, damage, knockback, player.whoAmI, player.direction * player.gravDir, 12, -MathHelper.PiOver2 * 0.5f);
            }
        }

        public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient<ArtemiteBar>(12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}