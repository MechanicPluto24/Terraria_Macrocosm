using Macrocosm.Content.Projectiles.Friendly.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons
{
    public class Colt : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dovah's Colt");
			Tooltip.SetDefault("Always shoots special bullets that redirect to nearby enemies after hitting");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 200;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 70;
			Item.height = 24;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 4f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Purple;
			Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
			//Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item38;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<ColtProjectile>();
		}
	}
}
