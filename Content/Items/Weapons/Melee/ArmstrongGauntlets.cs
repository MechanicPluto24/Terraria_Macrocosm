using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class ArmstrongGauntlets : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = DamageClass.Melee;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 19, copper: 69);
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.UseSound = SoundID.Item7;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArmstrongGauntletProjectile>();
            Item.shootSpeed = 20;
            Item.noUseGraphic = true;
        }
        int shots = 0;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 2;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity;
            Projectile.NewProjectileDirect(source, position, aim.RotatedByRandom(MathHelper.PiOver4 / 3), ModContent.ProjectileType<ArmstrongGauntletProjectile>(), damage, knockback, player.whoAmI, ai1: Item.shootSpeed, ai2: shots % 8 == 7 ? 1f : 0f);
            shots++;
            return false;
        }
    }
}