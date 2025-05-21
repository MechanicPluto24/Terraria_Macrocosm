using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class WaveGunBlue : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.UnobtainableItem[Type] = true;

            Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
            Redemption.AddElementToItem(Type, Redemption.ElementID.Celestial, true);
        }

        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<WaveGunBlueHeld>();
            Item.shootSpeed = 36f;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int fireRate = (int)(Item.useTime * player.GetAttackSpeed(DamageClass.Magic));
            Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<WaveGunBlueHeld>(), damage, knockback, player.whoAmI, ai0: fireRate);
            return false;
        }
    }
}
