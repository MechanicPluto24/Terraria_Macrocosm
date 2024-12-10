using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class Micronova : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 100000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MicronovaPortal>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {

            Projectile.NewProjectileDirect(source, position + new Vector2(Main.rand.NextFloat(-250f, 250f), Main.rand.NextFloat(-250f, 250f)), velocity, ModContent.ProjectileType<MicronovaPortal>(), damage, knockBack, player.whoAmI, ai1: -1);
            Projectile.NewProjectileDirect(source, position + new Vector2(Main.rand.NextFloat(-250f, 250f), Main.rand.NextFloat(-250f, 250f)), velocity, ModContent.ProjectileType<MicronovaPortal>(), damage, knockBack, player.whoAmI, ai1: 1);

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
    }
}