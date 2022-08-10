using Macrocosm.Content.Projectiles.Friendly.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons
{
    public class ArchersLine : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Archer's Line");
			Tooltip.SetDefault("Bullets ricochet from one enemy to another");
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
			position += new Vector2(0, -5); // muzzle offset 
			type = ModContent.ProjectileType<ArchersLineProjectile>();
		}

		public override Vector2? HoldoutOffset() => new Vector2(-14, 0);
	}
}
