using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class LHBMinigun : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LastPrism);
            Item.width = 74; // hitbox width of the item
            Item.height = 32; // hitbox height of the item
            Item.useTime = 7; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 7; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the item (swinging, holding out, etc)
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.knockBack = 0; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = Item.sellPrice(silver: 700);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<LHBMinigunProjectile>(); //this gun uses a holdout projectile
            Item.shootSpeed = 32f; // the speed of the projectile (measured in pixels per frame)
            Item.DamageType = DamageClass.Ranged; //deals melee damage
            Item.damage = 45; //the damage of your gun
            Item.reuseDelay = 8;
            Item.useAmmo = AmmoID.Bullet; //uses bullets as ammunition
            Item.mana = 0;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LHBMinigunProjectile>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
