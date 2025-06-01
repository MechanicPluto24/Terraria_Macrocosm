using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class FrigorianGaze : ModItem
    {
        public override void SetStaticDefaults()
        {
            Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
            Redemption.AddElementToItem(Type, Redemption.ElementID.Ice, true);
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 40;
            Item.height = 30;
            Item.useTime = 40;
            Item.useAnimation = 30;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity3>();
            Item.UseSound = SoundID.Item106;
            Item.shoot = ModContent.ProjectileType<FrigorianGazeProjectile>();
            Item.noUseGraphic = true;
            Item.shootSpeed = 10f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<FrigorianGazeProjectile>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
